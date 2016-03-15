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
using System.Net.Mail;
using System.Threading.Tasks;

namespace EPMSAppDemo.Controllers
{
    public class PerformancesController : Controller
    {
        private EPMSDevEntities db = new EPMSDevEntities();

        public async Task<ActionResult> email(FormCollection form, int Id)
        {
            var getRecordEmployee = db.Records.First(i => i.Id == Id).Record_Employee;
            var getGrade = db.Performances.First(i => i.RecordId == Id).Status;
            var getEmployeeName = db.Employees.First(i => i.Id == getRecordEmployee).FirstName;
            var name = form["sname"];
            var email = form["semail"];
            var messages = "Dear " + getEmployeeName + ",\n \n A recent record has been graded as " + getGrade + " by your manager. Please see http://epmsappdemo.azurewebsites.net/ and log on to the system using your details to view your performance page.";
            var phone = form["sphone"];
            var x = await SendEmail(name, email, messages, phone, Id);
            if (x == "sent")
            ViewData["esent"] = "Your Message Has Been Sent";
            return RedirectToAction("RecordIndex", "Employees", new { id = getRecordEmployee });
        }
        private async Task<String> SendEmail(string name, string email, string messages, string phone, int Id)
        {
            var getRecordEmployee = db.Records.First(i => i.Id == Id).Record_Employee;
            var getEmployeeEmail = db.Employees.First(i => i.Id == getRecordEmployee).UserName;
            var getEmployeeName = db.Employees.First(i => i.Id == getRecordEmployee).FirstName;
            var message = new MailMessage();
            message.To.Add(new MailAddress(getEmployeeEmail));  // replace with receiver's email id  
            message.From = new MailAddress("EPMSdonotreply@outlook.com");  // replace with sender's email id 
            message.Subject = "EPMS Email Notification";
            message.Body = messages;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "EPMSdonotreply@outlook.com",  // replace with sender's email id 
                    Password = "Txtekgsn1"  // replace with password 
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.live.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                return "sent";
            }
        }

        public static bool isManager()
        {
            EPMSDevEntities db = new EPMSDevEntities();

            //Get username from the browser and use it to query the db
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

        // GET: Performances
        public ActionResult Index()
        {
            var username = @System.Web.HttpContext.Current.User.Identity.Name;
            //get the current user's id
            var getUser = db.Employees.First(i => i.UserName == username).Id;

            //return the current user's performance pages using the id 
            var performances = db.Performances.Where(i => i.EmployeeId == getUser);
            
            return View(performances.ToList());
        }

        // GET: Performances/Details/5
        public ActionResult Details(int id)
        {
         
            var getPerformance = db.Performances.First(i => i.RecordId == id).Id;

            Performance performance = db.Performances.Find(getPerformance);
            if (performance == null)
            {
                return HttpNotFound();
            }
            return View(performance);
        }

        // GET: Performances/Create
        public ActionResult Create(int id)
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName");
            var empId = db.Works.First(i => i.Work_Record == id).Record.Record_Employee;
            Performance performance = new Performance();
            performance.RecordId = id;
            performance.EmployeeId = empId;
            
            return View(performance);
        }

        // POST: Performances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Justification,Aims,Grading,Status,EmployeeId,RecordId")] Performance performance)
        {
            if (ModelState.IsValid)
            {
                performance.Id = db.Performances.Max(i => i.Id + 1);
                db.Performances.Add(performance);
                db.SaveChanges();
                return RedirectToAction("UpdateRecord", "Records", new { id = performance.RecordId });
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", performance.EmployeeId);
            return View(performance);
        }

        // GET: Performances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", performance.EmployeeId);
            return View(performance);
        }

        // POST: Performances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Justification,Aims,Grading,Status,EmployeeId")] Performance performance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(performance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FirstName", performance.EmployeeId);
            return View(performance);
        }

        // GET: Performances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            return View(performance);
        }

        // POST: Performances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Performance performance = db.Performances.Find(id);
            db.Performances.Remove(performance);
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
