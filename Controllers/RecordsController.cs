using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EPMSAppDemo.Models;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace EPMSAppDemo.Controllers
{
    public class RecordsController : Controller
    {
        private EPMSDevEntities db = new EPMSDevEntities();

        // Use this static method to check if the logged in user is a team leader
        public static bool isManager()
        {
            EPMSDevEntities db = new EPMSDevEntities();

            //Get user name from the browser and use it to query the db
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            if (username != "")
            {
                //Get user's ID
                var userid = db.Employees.FirstOrDefault(i => i.UserName == username).Id;
                //get ismanager value
                var ismanager = false;//db.Records.First(i => i.Record_Employee == userid).IsTeamManager;
                //Search the teams db to see if the user is a manager
                var searchManager = db.Teams.Count(i => i.Manager == userid);

                //If the user is a manager then set the flag to be true
                if (searchManager > 0)
                {
                    ismanager = true;
                }

                return ismanager;
            }
            return false;
        }

        // GET: Records
        public ActionResult Index()
        {
            var username = @System.Web.HttpContext.Current.User.Identity.Name;

            if (username != "")
            {
                //get the current user's id
                var getUser = db.Employees.First(i => i.UserName == username).Id;

                //return the current user's performance pages using the id 
                var records = db.Records.Where(i => i.Record_Employee == getUser);


                return View(records.ToList());
            }
            return View();
        }

        // GET: Records/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // GET: Records/Create
        public ActionResult Create()
        {
            //get the current user's id
            var getUser = db.Employees.First(i => i.UserName == User.Identity.Name).Id;

            var records = db.Records.Where(i => i.Record_Employee == getUser);

            var noRecords = db.Records.Count(i => i.Record_Employee == getUser);

            var startDate = DateTime.Now;

            if (noRecords == 0)
            {


                startDate = DateTime.Now;
            }
            else if (noRecords > 0)
            {
                var getFirstRecord = records.Max(i => i.Id);
                var getPreviousRecordTimeBegin = records.First(i => i.Record_Employee == getUser && i.Id == getFirstRecord).TimePeriodBegin;
                var getPreviousRecordTimeEnd = records.First(i => i.Record_Employee == getUser && i.Id == getFirstRecord).TimePeriodEnd;
                startDate = getPreviousRecordTimeBegin.AddMonths(3);
            }







            Record record = new Record()
            {
                //Get the values of the current work and insert into the work created
                //WorkDate = work.WorkDate,

                Id = db.Records.Max(i => i.Id + 1),
                Status = "Open",
                TimePeriodBegin = startDate,
                TimePeriodEnd = startDate.AddMonths(3),
                Record_Employee = getUser,
                SubmittedDate = DateTime.Now
            };
            return View(record);
        }

        // POST: Records/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Record record)
        {

            record.Status = "Open";

            try
            {
                if (ModelState.IsValid)
                {
                    db.Records.Add(record);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException e)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = e.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, e.EntityValidationErrors);
            }

            return View(record);
        }

        // GET: Records/Edit/5
        public ActionResult UpdateRecord(int id)
        {
            var getNewStatus = db.Performances.First(i => i.RecordId == id).Status;

            Record record = db.Records.Find(id);



            record.Status = getNewStatus;


            if (record == null)
            {
                return HttpNotFound();
            }
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RecordIndex", "Employees", new { id = record.Record_Employee });
        }

        // GET: Records/Edit/5
        public ActionResult SubmitRecord(int id)
        {

            Record record = db.Records.Find(id);


            record.Status = "Submitted";


            if (record == null)
            {
                return HttpNotFound();
            }
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Records/Edit/5
        public ActionResult Edit(int id)
        {

            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status,SubmittedDate,TimePeriodBegin,TimePeriodEnd,RowVersion,Record_Employee")] Record record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(record);
        }

        // GET: Records/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // POST: Records/Delete/5
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}