using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EPMSAppDemo.Models;

namespace EPMSAppDemo.Controllers
{
    //Controller that is used to manage the submitted records screen that managers can see when they login
    //Inherits methods from a Controller Abstract Class
    public class AdminRecordsController : Controller
    {
        private EPMSDevEntities db = new EPMSDevEntities();

        //
        // GET: /AdminRecords/

        public ActionResult Index()
        {
            //This is the integer value for the current user's team
            var userTeam = db.Employees.First(i => i.UserName == @User.Identity.Name).Team;
            //Get the Id of the logged in user
            var userId = db.Employees.First(i => i.UserName == @User.Identity.Name).Id;

            //Pull records from employees that this user managers
            var records = db.Records.Where(i => i.Employee.Team == userTeam && i.Status == "Submitted" || i.Employee.Employee_Employee == userId && i.Status == "Submitted");

            //Return them as a list in the view which is then rendered into a table in the view
            return View(records.Where(i => i.Employee.UserName != @User.Identity.Name).ToList());
        }

        //
        // GET: /AdminRecords/Details/5

        public ActionResult Details(int id = 0)
        {
            //Find the record in the database using the id
            Record timesheet = db.Records.Find(id);
            if (timesheet == null)
            {
                return HttpNotFound();
            }
            return View(timesheet);
        }

        //
        // GET: /AdminREcords/Create
        
        public ActionResult Create()
        {
            ViewBag.TimeSheet_Employee = new SelectList(db.Employees, "Id", "FirstName");
            return View();
        }

        //
        // POST: /AdminRecords/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Record record)
        {
            if (ModelState.IsValid)
            {
                db.Records.Add(record);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TimeSheet_Employee = new SelectList(db.Employees, "Id", "FirstName", record.Record_Employee);
            return View(record);
        }

        //
        // GET: /AdminRecords/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.TimeSheet_Employee = new SelectList(db.Employees, "Id", "FirstName", record.Record_Employee);
            return View(record);
        }

        //
        // POST: /AdminTimeSheets/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Record record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Record_Employee = new SelectList(db.Employees, "Id", "FirstName", record.Record_Employee);
            return View(record);
        }

        //
        // GET: /AdminRecords/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        //
        // POST: /AdminRecords/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Record record = db.Records.Find(id);
            db.Records.Remove(record);
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
