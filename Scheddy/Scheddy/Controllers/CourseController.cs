using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheddy.Models;
using System.Web.Mvc;

namespace Scheddy.Controllers
{
    public class CourseController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateCourse()
        {


        }

        public void DeleteCourse()
        {

        }

        public void AddCourse()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if(_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}