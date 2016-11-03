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
            var model = _db.Classrooms.ToList();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var classroom = _db.Classrooms.Single(r => r.ClassroomId == id);
            return View(classroom);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var classroom = _db.Classrooms.Single(r => r.ClassroomId == id);
            if (TryUpdateModel(classroom))
            {
                // save to database
                return RedirectToAction("Index");
            }
            return View(classroom);
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