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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb db = new ScheddyDb();
        public ActionResult Index()
        { 
            var schedules = new List<Schedule>();
            try
            {
                schedules = db.Schedules.ToList();
            }
            catch(Exception)
            {   
            }
            return View(schedules);
        }

        public ActionResult IndexByClassroom()
        {
            var model =
                from c in db.Classrooms
                join s in db.Sections on
                c.ClassroomId equals s.ClassroomId
                orderby c.ClassroomId, s.DaysTaught, s.StartTime
                select new ViewModels.ClassroomByTime
                {
                    BldgCode = c.BldgCode,
                    RoomNumber = c.RoomNumber,
                    DaysTaught = s.DaysTaught,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
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

            var model = from i in db.Instructors
                        join s in db.Sections on
                        i.InstructorId equals s.InstructorId
                        join c in db.Classrooms on 
                        s.ClassroomId equals c.ClassroomId
                        orderby i.LastName, i.FirstName
                        select new ViewModels.ScheduleInstructorSection
                        {
                            FirstName = i.FirstName,
                            LastName = i.LastName,
                            BldgCode = c.BldgCode,
                            RoomNumber = c.RoomNumber,
                            DaysTaught = s.DaysTaught,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime
                        };
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
            }
            catch (Exception)
            {
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

        public ActionResult ExportToExcel(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            var sections = db.Sections.Where(section => section.ScheduleId == id);
            
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("Credits", typeof(string));
            table.Columns.Add("OVRL", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Hrs Rg", typeof(string));
            table.Columns.Add("Day", typeof(string));
            table.Columns.Add("Course", typeof(string));
            table.Columns.Add("Start Time", typeof(string));
            table.Columns.Add("End Time", typeof(string));

            foreach (var i in schedule.sections)
            {
                int hoursWorking = 0;
                foreach (Section section in schedule.sections)
                {
                    hoursWorking += section.Course.CreditHours;
                }
                hoursWorking += i.Instructor.HoursReleased;
                table.Rows.Add(hoursWorking.ToString(), "", i.Instructor.FirstName + " " + i.Instructor.LastName,
                    i.Instructor.HoursRequired.ToString(), "", "", "", "");
                foreach (var row in schedule.sections)
                {
                    table.Rows.Add("", "", "", row.Course.CreditHours, row.DaysTaught, row.Course.Prefix + row.Course.CourseNumber,
                        row.StartTime.Value.TimeOfDay.ToString(), row.EndTime.Value.TimeOfDay.ToString());
                }
            }
            
            var grid = new GridView();
            grid.DataSource = table;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + schedule.ScheduleName + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
    }
}