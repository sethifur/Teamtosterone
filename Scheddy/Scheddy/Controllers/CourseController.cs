using Scheddy.Models;
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

<<<<<<< HEAD
        public Course GetCourse()
        {

            return new Course();
        }

        public void SetCourse()
        {

=======
        protected override void Dispose(bool disposing)
        {
            if(_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
>>>>>>> fa75e824be5cc41c9f80c12429d1acd4f1eb90c4
        }

    }
}