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
                    EndTime = s.EndTime,
                    Campus = c.Campus,
                    FirstName = s.Instructor.FirstName,
                    LastName = s.Instructor.LastName,
                    Prefix = s.Course.Prefix,
                    CourseNumber = s.Course.CourseNumber
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
            ScheduleInstructorSection model = new ScheduleInstructorSection();

           //for (int i = 0; i < db.Sections.Count(); i++)
            //{
                var query = from ii in db.Instructors
                            join s in db.Sections on
                            ii.InstructorId equals s.InstructorId
                            join c in db.Classrooms on
                            s.ClassroomId equals c.ClassroomId
                            join co in db.Courses on
                            s.CourseId equals co.CourseId
                            orderby s.StartTime ascending
                            select new
                            { ii, s, c, co};
                            /*   FirstName = ii.FirstName,
                               LastName = ii.LastName,
                               BldgCode = c.BldgCode,
                               RoomNumber = c.RoomNumber,
                               DaysTaught = s.DaysTaught,
                               StartTime = s.StartTime,
                               EndTime = s.EndTime
                           };
                           */
                foreach (var item in query)
                {
                    model.indexByProfessor.Add(new indexByProfessor()
                    {
                        FirstName = item.ii.FirstName,
                        LastName = item.ii.LastName,
                        BldgCode = item.c.BldgCode,
                        RoomNumber = item.c.RoomNumber,
                        DaysTaught = item.s.DaysTaught,
                        StartTime = item.s.StartTime,
                        EndTime = item.s.EndTime,
                        Campus = item.c.Campus,
                        Prefix = item.co.Prefix,
                        CourseNumber = item.co.CourseNumber,
                        SectionId = item.s.SectionId

                    });
                //}

                //model.indexByProfessor.Add(item);
            }
            model.instructor = db.Instructors;                       


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
            table.Columns.Add("Instructor", typeof(string));
            table.Columns.Add("Course", typeof(string));
            table.Columns.Add("CRN", typeof(string));
            table.Columns.Add("Start Time", typeof(string));
            table.Columns.Add("End Time", typeof(string));
            table.Columns.Add("Day", typeof(string));
            table.Columns.Add("Room", typeof(string));
            table.Columns.Add("MAX", typeof(string));
            table.Columns.Add("HRS", typeof(string));
            table.Columns.Add("Camp", typeof(string));
            table.Columns.Add("Pay", typeof(string));
            table.Columns.Add("Load/OVRL", typeof(string));
            table.Columns.Add("Hrs Reg", typeof(string));

            System.Data.DataTable onlineTable = new System.Data.DataTable();
            onlineTable.Columns.Add("Instructor", typeof(string));
            onlineTable.Columns.Add("Course", typeof(string));
            onlineTable.Columns.Add("CRN", typeof(string));
            onlineTable.Columns.Add("ENRL", typeof(string));
            onlineTable.Columns.Add("AVAIL", typeof(string));
            onlineTable.Columns.Add("WLIST", typeof(string));
            onlineTable.Columns.Add("Pay", typeof(string));
            onlineTable.Columns.Add("Load/OVRL", typeof(string));
            onlineTable.Columns.Add("", typeof(string));

            //Table that puts info into Regular table
            Instructor prevProf = null;
            foreach (var i in schedule.sections)
            {
                int hoursWorking = 0;
                foreach (Section section in schedule.sections)
                {
                        hoursWorking += section.Course.CreditHours;
                }
                if (i.DaysTaught == "ONL")
                {
                    prevProf = null;
                    foreach (var j in schedule.sections)
                    {
                        if (prevProf != j.Instructor)
                        {
                            hoursWorking += j.Instructor.HoursReleased;
                            onlineTable.Rows.Add(i.Instructor.LastName + ", " + j.Instructor.FirstName, "", "", "", "", "", "", "",
                                "", "", "", "", "");
                            foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == j.Instructor.InstructorId))
                            {
                                onlineTable.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, row.CRN,
                                    "", row.Classroom.Capacity, "", "", "", "");
                            }
                        }
                        prevProf = j.Instructor;
                    }
                }
                else
                {
                    if (prevProf != i.Instructor)
                    {

                        hoursWorking += i.Instructor.HoursReleased;
                        table.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "",
                            "", "", "", "", i.Instructor.HoursRequired);
                        foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == i.Instructor.InstructorId))
                        {
                            table.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, "",
                                row.StartTime.Value.TimeOfDay.ToString(), row.EndTime.Value.TimeOfDay.ToString(), row.DaysTaught,
                                row.Classroom.BldgCode + " " + row.Classroom.RoomNumber, row.numSeats, row.Course.CreditHours,
                                row.Classroom.Campus, "", "", "");
                        }
                    }
                    prevProf = i.Instructor;
                }
            }
            
           

            var grid1 = new GridView();
            grid1.DataSource = table;
            grid1.DataBind();

            var grid2 = new GridView();
            grid2.DataSource = onlineTable;
            grid2.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + schedule.ScheduleName +
                "_" + schedule.Semester + "_" + schedule.AcademicYear + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid1.RenderControl(htw);
            grid2.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
    }
}