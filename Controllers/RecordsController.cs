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
        //Controller used to manage all the records in the system
        private EPMSDevEntities db = new EPMSDevEntities();

       /* public ActionResult CheckUser()
        {
            //Get user name from the browser and use it to query the db
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            var searchForUser = db.Employees.Count(i => i.UserName == username);
            if (searchForUser == 0)
{
                return RedirectToAction("Shared", "Error");
}
            return View();
        }*/

        // Use this static method to check if the logged in user is a team leader
        public static bool isManager()
        {
            EPMSDevEntities db = new EPMSDevEntities();

            //Get user name from the browser and use it to query the db
            var username = System.Web.HttpContext.Current.User.Identity.Name;

            var searchForUser = db.Employees.Count(i => i.UserName == username);
            if (username != "" && searchForUser != 0)
            {
                /*if (searchForUser == 0)
                {
                    RecordsController.Redirect();
                }*/
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

       // public static ActionResult Redirect()
       // {
         //   return RedirectToAction("Shared", "Error");
        //}

        // GET: Records
        public ActionResult Index()
        {
            //Get the current logged in user from the db
            var username = @System.Web.HttpContext.Current.User.Identity.Name;
            //See if they exist in the db
            var searchForUser = db.Employees.Count(i => i.UserName == username);

            if (username != "" && searchForUser != 0)
            {
                //get the current user's id
                var getUser = db.Employees.First(i => i.UserName == username).Id;

                //grab all records for this user
                var records = db.Records.Where(i => i.Record_Employee == getUser);

                //return a list of these records
                return View(records.ToList());


            } else if (searchForUser > 0)
            {
                return RedirectToAction("shared", "error");
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
            //get the records for the current user
            var records = db.Records.Where(i => i.Record_Employee == getUser);
            //Count the number of records
            var noRecords = db.Records.Count(i => i.Record_Employee == getUser);
            //Get the date begin for the record (now)
            var startDate = DateTime.Now;
            //If no records exist then the start date is now
            if (noRecords == 0)
            {
            startDate = DateTime.Now;
            }
            //If there are already records for the user then the start date must be for the next three months
            else if (noRecords > 0)
            {
                var getFirstRecord = records.Max(i => i.Id);
                var getPreviousRecordTimeBegin = records.First(i => i.Record_Employee == getUser && i.Id == getFirstRecord).TimePeriodBegin;
                var getPreviousRecordTimeEnd = records.First(i => i.Record_Employee == getUser && i.Id == getFirstRecord).TimePeriodEnd;
                startDate = getPreviousRecordTimeBegin.AddMonths(3);
            }
            //Create the new record that will be passed to the post method
             Record record = new Record()
            {
                //Set all values of new record
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Record record)
        {
            record.Status = "Open";
            //Start of try catch method to add the record to the database
            try
            {
                if (ModelState.IsValid)
                {
                    db.Records.Add(record);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            //If the error is caught then display meaningful error message (as well as custom error page)
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
            return RedirectToAction("email", "Performances", new { id = record.Id });
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
            return RedirectToAction("Index", "Employees");
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