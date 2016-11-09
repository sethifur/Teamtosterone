using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scheddy.Models;


namespace Scheddy.Controllers
{
    public class HomeController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        //default controller. this is for the home page
        public ActionResult Index()
        {  
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}