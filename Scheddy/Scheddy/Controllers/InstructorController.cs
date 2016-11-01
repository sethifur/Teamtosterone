using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scheddy.Models;

namespace Scheddy.Controllers
{
    public class InstructorController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Instructor
        public ActionResult Index()
        {
            return View();
        }

<<<<<<< HEAD
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

=======
        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
>>>>>>> fa75e824be5cc41c9f80c12429d1acd4f1eb90c4
        }
    }
}