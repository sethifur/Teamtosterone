using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Scheddy.ViewModels;

namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        //list of schedules
        public ActionResult Index()
        {
            var schedules = db.Schedules.ToList();
            return View(schedules);   
        }

        public ActionResult IndexByClassroom()
        {
            var model =
                from c in db.Classrooms
                join s in db.Sections on
                c.ClassroomId equals s.ClassroomId
                orderby c.ClassroomId, s.StartTime
                select new ViewModels.ClassroomByTime
                {
                    BldgCode = c.BldgCode,
                    RoomNumber = c.RoomNumber,
                    //StartTime = s.StartTime,
                    //EndTime = s.EndTime
                };
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            return View(schedule);
        }

        public ActionResult IndexByProfessor()
        {

            var model =
                            from i in db.Instructors
                            join s in db.Sections on
                            i.InstructorId equals s.InstructorId
                            orderby i.LastName, i.FirstName
                            select new ViewModels.ScheduleInstructorSection
                            {
                                FirstName = i.FirstName,
                                LastName = i.LastName,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime
                            };

            List<Section> sections = new List<Section>();

        
               /* var section =
                            from i in db.Instructors
                            join s in db.Sections on
                            i.InstructorId equals s.InstructorId
                            orderby i.LastName, i.FirstName
                            select new List<Section> 
                            {
                                
                            };
                */


           

            return View(model);
        }

        public ActionResult Create()
        {

            return View();
        }

        public ActionResult CannotDelete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Semester,AcademicYear,ScheduleName,DateCreated,DateModified,CreatedBy,UpdatedBy")] Schedule schedule)
        {
            try
            {
                db.Schedules.Add(schedule);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UpdateSchedule(List<Section> sections)
        {    
            return View();
        }

        public ActionResult Delete(int? id)
        {
            //was there an id passed?
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //grab the schedule
            Schedule schedule = db.Schedules.Find(id);

            //does that schedule exist?
            if (schedule == null)
            {
                return HttpNotFound();
            }

            //loop through and delete scheduleId's from section table.
            var sections = db.Sections.Where(section => section.ScheduleId == id).ToList();

            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schedule schedule = db.Schedules.Find(id);

            try
            {
                db.Schedules.Remove(schedule);
                db.SaveChanges();
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION scheduleController: " + e.Message);
                return RedirectToAction("CannotDelete");
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetSchedule(int? id)
        {
            //was an id passed in?
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //grab schedule to return
            Schedule schedule = db.Schedules.Find(id);

            //does it exist?
            if (schedule == null)
            {
                return HttpNotFound();
            }

            //return it
            return View(schedule);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}