using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPMSAppDemo.Models;

namespace EPMSAppDemo.Controllers
{
    public class WorksController : Controller
    {
        //Create a new instance of the database to allow us to access it
        private EPMSDevEntities db = new EPMSDevEntities();



        //
        // GET: /TimeTrackerDev4/
        [HttpGet]
        public ActionResult Index(int id = 0)
        {
            //get username
            var username = @User.Identity.Name;

            //var test = db.TimeEntries.Where(i => i.TimeSheet.Employee.UserName == username)
            //.Where(i => i.TimeSheet.Id == timesheet.Id);
            //    //.Where(i => i.TimeSheet.TimeSheet_Employee == timeentry.TimeSheet.TimeSheet_Employee)
            //    //.Where(i => i.TimeEntry_TimeSheet == timeentry.TimeEntry_TimeSheet);


            //var m = db.TimeEntries.Where(i => i.TimeSheet.Employee.UserName == username)
            //   .Where(i => i.TimeSheet.TimeSheet_Employee == testID1)
            //   .Where(i => i.TimeEntry_TimeSheet == testID2);

            //This query took a bit of time to produce and the issue was getting the ID of the time sheet to pass through 
            //to the time entries view. My mistake was in the 'entries' URL: when creating this I had the ID and the null
            //value round the wrong way.

            //Get the time entries from the database
            var entries = db.Works.Where(i => i.Record.Employee.UserName == username && i.Work_Record == id)
                .OrderByDescending(i => i.DateCompleted);

            var timeEntries = new PiecesOfWork
            {
                RecordId = id,
                Entries = entries
            };

            return View(timeEntries);


        }

        //
        // GET: Get the values for the copy row function

        public ActionResult Details(int id = 0)
        {
            //Get the time entry selected using the ID
            Work work = db.Works.Find(id);

            var timebegin = db.Records.First(i => i.Id == work.Work_Record).TimePeriodBegin;
            var timeend = db.Records.First(i => i.Id == work.Work_Record).TimePeriodEnd;

            //Create the new time entry
            Work work1 = new Work()
            {
                //Get the values of the current time entry and insert into the time entry created
                //WorkDate = timeentry.WorkDate,
                WorkItem = work.WorkItem,
                Description = work.Description,
                HoursWorked = work.HoursWorked,
                HoursRemaining = work.HoursRemaining,
                Work_Project = work.Work_Project,
                Work_Category = work.Work_Category,
                tempTimeBegin = timebegin,
                tempTimeEnd = timeend,
                Id = work.Work_Record,
                Work_Record = work.Work_Record,
                SubmittedDate = DateTime.Now
            };

            if (work == null)
            {
                return HttpNotFound();
            }
            return View(work1);
        }

        // POST: Copy row funtion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Work work)
        {
            if (ModelState.IsValid)
            {
                db.Works.Add(work);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = work.Work_Record });
            }

            ViewBag.Work_Category = new SelectList(db.Categories.Where(i => i.IsActive == true).OrderBy(x => x.Name), "Id", "Name", work.Work_Category);
            ViewBag.Work_Project = new SelectList(db.Projects.OrderBy(x => x.Name), "Id", "Name", work.Work_Project);
            ViewBag.Work_Record = new SelectList(db.Records, "Id", "Status", work.Work_Record);
            return View(work);
        }

        //Filling in the project dropdowns depending on which team the user is in
        public void ProjectList()
        {

            //in order to get the dropdown data for the projects we must get the team the user belongs to
            var userTeam = db.Employees.First(i => i.UserName == @User.Identity.Name).Team1.TeamName;

            //Fill project dropdowns for development team
            if (userTeam == "Development")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.DevTeam == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for service desk team
            else if (userTeam == "Service Desk")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.ServiceDesk == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for operations team
            else if (userTeam == "Operations")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.Operations == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for comms team
            else if (userTeam == "Communications")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.Communications == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for local support west team
            else if (userTeam == "Local Support West")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.LocalSupportWest == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for local support east team
            else if (userTeam == "Local Support East")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.LocalSupportEast == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for local support north team
            else if (userTeam == "Local Support North")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.LocalSupportNorth == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for admin team
            else if (userTeam == "Admin")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.Admin == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for trainers team
            else if (userTeam == "Trainers")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.Trainers == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for business services delivery team
            else if (userTeam == "Business Services Delivery")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.BusinessServicesDelivery == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
            //Fill project dropdowns for business services analysis team
            else if (userTeam == "Business Services Analysis")
            {
                ViewBag.Work_Project = new SelectList(db.Projects.Where(i => i.BusinessServicesAnalysis == true && i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            }
        }

        //
        // GET: /TimeTrackerDev4/Create

        public ActionResult Create(int id = 0)
        {
            //Call method to populate the dropdown menus
            ProjectList();

            ViewBag.Work_Category = new SelectList(db.Categories.Where(i => i.IsActive == true).OrderBy(x => x.Name), "Id", "Name");
            ViewBag.Work_TimeSheet = new SelectList(db.Records, "Id", "Status");

            var timebegin = db.Records.First(i => i.Id == id).TimePeriodBegin;
            var timeend = db.Records.First(i => i.Id == id).TimePeriodEnd;


            //Create the new time entry
            Work work = new Work()
            {
                tempTimeBegin = timebegin,
                tempTimeEnd = timeend,
                Id = -1,
                Work_Record = id,
                HoursWorked = 0,
                SubmittedDate = DateTime.Now
            };



            return View(work);
        }

        //
        // POST: /TimeTrackerDev4/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Work work)
        {
            if (ModelState.IsValid)
            {
                
                if(work.DateCompleted > work.DateDue)
                {

                    work.Late = 1;
                    
                } else
                {
                    work.Late = 0;
                }
                
                work.HoursWorked = 0;
                work.Id = db.Works.Max(i => i.Id + 1); 
                db.Works.Add(work);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = work.Work_Record });
            }

            ViewBag.Work_Category = new SelectList(db.Categories.Where(i => i.IsActive == true).OrderBy(x => x.Name), "Id", "Name", work.Work_Category);
            ViewBag.Work_Project = new SelectList(db.Projects.OrderBy(x => x.Name), "Id", "Name", work.Work_Project);
            ViewBag.Work_TimeSheet = new SelectList(db.Records, "Id", "Status", work.Work_Record);
            return View(work);
        }

        //
        // GET: /TimeTrackerDev4/Edit/5

        public ActionResult Edit(int id = 0)
        {

            //Find the time entry that the user wants to edit using the ID
            Work work = db.Works.Find(id);

            var timebegin = db.Records.First(i => i.Id == work.Work_Record).TimePeriodBegin;
            var timeend = db.Records.First(i => i.Id == work.Work_Record).TimePeriodEnd;

            work.tempTimeBegin = timebegin;
            work.tempTimeEnd = timeend;

            if (work == null)
            {
                return HttpNotFound();
            }
            ViewBag.Work_Category = new SelectList(db.Categories.Where(i => i.IsActive == true).OrderBy(x => x.Name), "Id", "Name", work.Work_Category);
            ProjectList();
            ViewBag.Work_Record = new SelectList(db.Records, "Id", "Status", work.Work_Record);
            return View(work);
        }

        //
        // POST: /TimeTrackerDev4/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Work work)
        {
            if (ModelState.IsValid)
            {

                //Modify the record and save the changes to the database
                db.Entry(work).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = work.Work_Record });
            }
            ViewBag.Work_Category = new SelectList(db.Categories.Where(i => i.IsActive == true).OrderBy(x => x.Name), "Id", "Name", work.Work_Category);
            ViewBag.Work_Project = new SelectList(db.Projects.OrderBy(x => x.Name), "Id", "Name", work.Work_Project);
            ViewBag.TimeEntry_TimeSheet = new SelectList(db.Records, "Id", "Status", work.Work_Record);
            return View(work);
        }

        //
        // GET: /TimeTrackerDev4/Delete/5

        public ActionResult Delete(int id = 0)
        {
            //Find the time entry the user wants to delete by using the ID
            Work work = db.Works.Find(id);
            if (work == null)
            {
                return HttpNotFound();
            }
            return View(work);
        }

        //
        // POST: /TimeTrackerDev4/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Work work = db.Works.Find(id);
            db.Works.Remove(work);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = work.Work_Record });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}