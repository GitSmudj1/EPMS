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
    public class EmployeesController : Controller
    {
        private EPMSDevEntities db = new EPMSDevEntities();

        // GET: /Employees/

        public ActionResult Index()
        {
            bool isTeamManager;

            //Get the employee ID using the logged in user
            var getEmployeeID = db.Employees.First(i => i.UserName == User.Identity.Name).Id;
            //Get the employee team using the employee ID
            var getTeam = db.Employees.First(i => i.Id == getEmployeeID).Team;
            //Get the manager using the employee team
            var getManager = db.Teams.First(i => i.Id == getTeam).Manager;
            //Get the ID of that manager
            var getManagerTeam = db.Teams.First(i => i.Id == getTeam).Id;
            //Check if the manager exists and return true
            if (getManager == getEmployeeID)
            {
                isTeamManager = true;
            }

            //Get all employees that are under this team manager
            var employees = db.Employees.Where(i => i.Team == getManagerTeam);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.Employee_Employee = new SelectList(db.Employees, "Id", "FirstName");
            Employee employee = new Employee();

            return View(employee);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //
        // POST: /Employees/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Team,UserName,FirstName,LastName,IsActive")]Employee employee)
        {
            //get the current user's team using the username
            var usersTeam = db.Employees.Single(i => i.UserName == @User.Identity.Name).Team1.TeamName;
            var usersTeamNum = db.Employees.Single(i => i.UserName == @User.Identity.Name).Team;
            //get the current user's id using the username
            var usersId = db.Employees.First(i => i.UserName == @User.Identity.Name).Id;

            if (ModelState.IsValid)
            {

                //employee.Team1.Id = db.Employees.First(i => i.UserName == @User.Identity.Name).Team;
                //employee.Team1.TeamName = usersTeam;
                //employee.Team = usersTeamNum;
                //db.Employees.Add(employee);

                employee.Employee_Employee = usersId;
                employee.Id = db.Employees.Max(i => i.Id + 1);
                employee.Team = usersTeamNum;

                db.Employees.Add(employee);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                return RedirectToAction("Index");
            }

            ViewBag.Employee_Employee = new SelectList(db.Employees, "Id", "FirstName", employee.Employee_Employee);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            var currentEmployeeId = db.Employees.First(i => i.UserName == @User.Identity.Name).Id;
            if (ModelState.IsValid)
            {

                employee.Employee_Employee = currentEmployeeId;
                employee.Team = employee.Team1.Id;
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        public ActionResult RecordIndex(int id = 0)
        {
      

            //get employee details using the employee id
            Employee employee = db.Employees.Find(id);

            //query the db and produce the list of records for the user
            var m = db.Records.Where(i => i.Employee.UserName == employee.UserName).OrderBy(i => i.TimePeriodBegin);

            //Get user name from the browser and use it to query the db
            var username = @User.Identity.Name;


            return View(m.ToList());
        }

        //
        // GET: Admin view for timeentries

        public ActionResult WorkIndex(int id = 0)
        {
            //get employee using the id parameter
            Employee employee = db.Employees.Find(id);
            //get value for employee id using the id parameter
            var employeeId = db.Records.First(i => i.Id == id).Record_Employee;
            //get the employee username using the employee id
            var employeeUsername = db.Employees.First(i => i.Id == employeeId).UserName;
            //grab all the pieces of work relevant to the specific employee
            var entries = db.Works.Where(i => i.Record.Employee.UserName == employeeUsername && i.Work_Record == id)
                .OrderByDescending(i => i.DateCompleted);

            var timeEntries = new PiecesOfWork
            {
                RecordId = id,
                Entries = entries
            };


            return View(timeEntries);
        }


        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
