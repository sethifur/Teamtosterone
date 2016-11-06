using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Scheddy.ViewModels;


namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        // GET: Schedule
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSchedule(ScheduleViewModel scheduleVm, List<Section> sections)
        {
            //build and insert to schedule table
            Schedule schedule = new Schedule();
            schedule.AcademicYear = scheduleVm.AcademicYear;
            schedule.Semester = scheduleVm.Semester;
            schedule.UpdatedBy = scheduleVm.UpdatedBy;
            schedule.ScheduleName = scheduleVm.ScheduleName;
            schedule.sections = sections;

            db.Schedules.Add(schedule);
            db.SaveChanges();

            int id = schedule.ScheduleId;
            //go through list of sections and add scheduleId and add them in section table.
            
            foreach (var section in sections)
            {
                section.ScheduleId = schedule.ScheduleId;
                db.Sections.Add(section);
            }
            
            return View(schedule);
        }

        public ActionResult UpdateSchedule(int? id, ScheduleViewModel scheduleVm, List<Section> sections)
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

            foreach (var section in sections)
            {
                section.ScheduleId = id;
                db.Sections.Add(section);
            }

            schedule.AcademicYear = scheduleVm.AcademicYear;
            schedule.Semester = scheduleVm.Semester;
            schedule.UpdatedBy = scheduleVm.UpdatedBy;
            schedule.ScheduleName = scheduleVm.ScheduleName;
            schedule.sections = sections;

            db.Schedules.Add(schedule);
            db.SaveChanges();
            return View(schedule);
        }

        public ActionResult DeleteSchedule(int? id)
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
        public Section CheckConflict()
        {
            return new Section();
        }

        public void SetSchedule()
        {
            //?? I'm not sure what this method is compared to AddSchedule()
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