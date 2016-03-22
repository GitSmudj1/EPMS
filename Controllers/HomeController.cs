using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPMSAppDemo.Controllers
{

    //Controller which manages all main pages
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Employee Performance Management System";

            return View();
        }

        public ActionResult Contact()
        {
           
            return View();
        }
    }
}