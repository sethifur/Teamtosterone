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
using System.Data;
using System.Reflection;

namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb db = new ScheddyDb();
        object missing = Type.Missing;
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

        public ActionResult IndexByClassroom(int? id)
        {

            ScheduleClassroomSection model = new ScheduleClassroomSection();

            var query = from ii in db.Instructors
                        join s in db.Sections on
                        ii.InstructorId equals s.InstructorId
                        join c in db.Classrooms on
                        s.ClassroomId equals c.ClassroomId
                        join co in db.Courses on
                        s.CourseId equals co.CourseId
                        orderby c.Campus, c.BldgCode, s.StartTime ascending
                        select new
                        { ii, s, c, co };
            foreach (var item in query)
            {
                model.indexByClassroom.Add(new indexByClassroom()
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
            }
            model.classroom = db.Classrooms;

            if (id != null)
            {
                model.scheduleId = id;
            }

            return View(model);
        }
        
        public ActionResult IndexByProfessor(int? id)
        {
            ScheduleInstructorSection model = new ScheduleInstructorSection();

            if (id != null)
            {
                model.scheduleId = id;
            }

            var query = from ii in db.Instructors
                        join s in db.Sections on
                        ii.InstructorId equals s.InstructorId
                        join c in db.Classrooms on
                        s.ClassroomId equals c.ClassroomId
                        join co in db.Courses on
                        s.CourseId equals co.CourseId
                        orderby s.StartTime ascending
                        select new
                        { ii, s, c, co };
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

        /*
        public ActionResult ExportToExcel(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            var sections = db.Sections.Where(section => section.ScheduleId == id);
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook worKbooK;
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
            Microsoft.Office.Interop.Excel.Range celLrangE;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;
                excel.DisplayAlerts = false;
                worKbooK = excel.Workbooks.Add(Type.Missing);


                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                worKsheeT.Name = "CS FTF_"+schedule.ScheduleName+"_"+schedule.Semester+"_"+schedule.AcademicYear;

                worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[1, 8]].Merge();
                worKsheeT.Cells[1, 1] = "Courses by Professors";
                worKsheeT.Cells.Font.Size = 15;


                int rowcount = 2;

                foreach (DataRow datarow in Excel(id).Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= Excel(id).Columns.Count; i++)
                    {

                        if (rowcount == 3)
                        {
                            worKsheeT.Cells[2, i] = Excel(id).Columns[i - 1].ColumnName;
                            worKsheeT.Cells.Font.Color = System.Drawing.Color.Black;

                        }

                        worKsheeT.Cells[rowcount, i] = datarow[i - 1].ToString();

                        if (rowcount > 3)
                        {
                            if (i == Excel(id).Columns.Count)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    celLrangE = worKsheeT.Range[worKsheeT.Cells[rowcount, 1], worKsheeT.Cells[rowcount, Excel(id).Columns.Count]];
                                }

                            }
                        }

                    }

                }

                celLrangE = worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[rowcount, Excel(id).Columns.Count]];
                celLrangE.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = celLrangE.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;

                celLrangE = worKsheeT.Range[worKsheeT.Cells[1, 1], worKsheeT.Cells[2, Excel(id).Columns.Count]];

                worKbooK.SaveAs("~/Downloads/"+schedule.ScheduleName+ "_" + schedule.Semester + "_" + schedule.AcademicYear + ".xls"); ;
                worKbooK.Close();
                excel.Quit();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                worKsheeT = null;
                celLrangE = null;
                worKbooK = null;
            }
            return View();
        }
        */
        public System.Data.DataTable Excel(int? id)
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



            //Table that puts info into Regular table
            Instructor prevProf = null;
            foreach (var i in schedule.sections)
            {
                int hoursWorking = 0;
                foreach (Section section in schedule.sections)
                {
                    hoursWorking += section.Course.CreditHours;
                }
                if (i.DaysTaught != "ONL")
                {
                    if (prevProf != i.Instructor)
                    {

                        hoursWorking += i.Instructor.HoursReleased;
                        table.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "",
                            "", "", "", "", i.Instructor.HoursRequired);
                        foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == i.Instructor.InstructorId))
                        {
                            table.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, "", row.StartTime.Value.TimeOfDay.ToString(),
                                row.EndTime.Value.TimeOfDay.ToString(), row.DaysTaught, row.Classroom.BldgCode + " " + row.Classroom.RoomNumber,
                                row.numSeats, row.Course.CreditHours, row.Classroom.Campus, "", "", "");
                        }
                    }
                    prevProf = i.Instructor;
                }
            }

            return table;

            //return View("MyView");
        }

        public System.Data.DataTable getOnlineExcel(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            var sections = db.Sections.Where(section => section.ScheduleId == id);

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
                            onlineTable.Rows.Add(i.Instructor.LastName + ", " + j.Instructor.FirstName, "", "", "", "", "", "", "", "");
                            foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == j.Instructor.InstructorId))
                            {
                                onlineTable.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, row.CRN, "",
                                    row.Classroom.Capacity, "", "", "", "");
                            }
                        }
                        prevProf = j.Instructor;
                    }
                }
            }
            return onlineTable;
        }
    }
}

