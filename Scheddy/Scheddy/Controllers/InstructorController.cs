using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Scheddy.Controllers
{
    public class InstructorController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Instructor
        public ActionResult Index()
        {
            var model = _db.Instructors.ToList();
            return View(model);
        }

        public void UpdateInstructor()
        {


        }

        public void DeleteInstructor()
        {

        }

        public void AddInstructor()
        {

        }

        public Instructor GetInstructor()
        {

            return new Instructor();
        }

        public void SetInstructor()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}