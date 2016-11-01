using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheddy.Controllers
{
    public class ClassroomController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Classroom
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateClassroom()
        {


        }
        public void DeleteClassroom()
        {

        }
        public void AddClassroom()
        {

        }

        public Classroom GetClassroom()
        {

            return new Classroom();
        }

        public void SetClassroom()
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