using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPMSAppDemo.Models;

namespace TimeTracker.Controllers
{
    public class ProjectsController : Controller
    {
        //Controller to manage the projects in the system
        private EPMSDevEntities db = new EPMSDevEntities();

        //
        // GET: /EPMSDev2/

        public ActionResult Index()
        {
            var projs = db.Projects.OrderBy(m => m.Name);

            return View(projs.ToList());
        }

        //
        // GET: /EPMSDev2/Details/5

        public ActionResult Details(int id = 0)
        {
            //grab the project using the id that is passed in
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        //
        // GET: /EPMSDev2/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /EPMSDev2/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            //Create the new project and add into the database
            if (ModelState.IsValid)
            {
                project.Id = db.Projects.Max(i => i.Id + 1);
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        //
        // GET: /APMSDev2/Edit/5

        public ActionResult Edit(int id = 0)
        {
            //Grab the project using the id that is passed in
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        //
        // POST: /EPMSDev2/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            //Save the changes made
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        //
        // GET: /EPMSDev2/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        //
        // POST: /EPMSDev2/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}