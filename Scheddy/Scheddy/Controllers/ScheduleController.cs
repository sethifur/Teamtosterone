using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        //list of schedules
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexByClassroom()
        {
            return View();
        }

        public ActionResult IndexByProfessor()
        {
            return View();
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

            //delete schedule.

            db.Schedules.Remove(schedule);
            db.SaveChanges();
            return View();
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

        //this should probably be a Javascript function on the view when it's made.
        public int CheckConflict(int? scheduleId, Section newSection)
        {
            Schedule schedule = db.Schedules.Find(scheduleId);
            
            if (schedule.sections.Count == 0)
            {
                return 0;
            }
            
            foreach (Section section in schedule.sections)
            {
                // classrooms at same time conflict
                if (section.Classroom == newSection.Classroom)
                {
                    // check for overlapping times
                    if (section.StartDate < newSection.EndTime && newSection.StartTime < section.EndTime)
                    {
                        return 1;
                    }
                }

                // instructors at same time conflict
                if (section.Instructor == newSection.Instructor)
                {
                    // check for overlapping times
                    if (section.StartDate < newSection.EndTime && newSection.StartTime < section.EndTime)
                    {
                        return 2;
                    }
                }

                // course has another section at same time conflict
                if (section.Course == newSection.Course)
                {
                    // check for overlapping times
                    if (section.StartDate < newSection.EndTime && newSection.StartTime < section.EndTime)
                    {
                        return 3;
                    }
                }
            }

            // intructor overworking conflict
            int hoursWorking = 0;
            foreach (Section section in newSection.Instructor.sections)
            {
                hoursWorking += section.Course.CreditHours;
            }
            if (hoursWorking > newSection.Instructor.HoursRequired)
            {
                return 4;
            }
            
            // if we've gotten here then no conflicts were found.
            return 0;
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