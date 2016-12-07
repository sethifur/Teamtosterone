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

        
        public ActionResult ExportToExcel(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            var sections = db.Sections.Where(section => section.ScheduleId == id);
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workBook;
            Microsoft.Office.Interop.Excel.Worksheet workSheet;
            Microsoft.Office.Interop.Excel.Worksheet workSheet2;
            Microsoft.Office.Interop.Excel.Range cellRange;
            Microsoft.Office.Interop.Excel.Range cellRange2;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;
                excel.DisplayAlerts = false;
                workBook = excel.Workbooks.Add(Type.Missing);
                
                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;
                workSheet.Name = "CS FTF_"+schedule.Semester+"_"+schedule.AcademicYear;
                workSheet.Cells.Font.Size = 12;

                int rowcount = 2;

                foreach (DataRow datarow in Excel(id).Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= Excel(id).Columns.Count; i++)
                    {
                        if (rowcount == 3)
                        {
                            workSheet.Cells[1, i] = Excel(id).Columns[i - 1].ColumnName;
                            workSheet.Cells.Font.Color = System.Drawing.Color.Black;
                        }

                        workSheet.Cells[rowcount, i] = datarow[i - 1].ToString();

                        if (rowcount > 3)
                        {
                            if (i == Excel(id).Columns.Count)
                            {
                                //if (rowcount % 2 == 0)
                                //{
                                    cellRange = workSheet.Range[workSheet.Cells[rowcount, 1]
                                        , workSheet.Cells[rowcount, Excel(id).Columns.Count]];
                                //}
                            }
                        }
                    }
                }
                cellRange = workSheet.Range[workSheet.Cells[1, 1],
                    workSheet.Cells[rowcount, Excel(id).Columns.Count]];
                cellRange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = cellRange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[2, Excel(id).Columns.Count]];


                workSheet2 = workBook.Sheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                workSheet2.Name = "CS Onl_" + schedule.Semester + "_" + schedule.AcademicYear;
                workSheet2.Cells.Font.Size = 12;

                int rowcount2 = 2;

                foreach (DataRow datarow in getOnlineExcel(id).Rows)
                {
                    rowcount2 += 1;
                    for (int i = 1; i <= getOnlineExcel(id).Columns.Count; i++)
                    {
                        if (rowcount2 == 3)
                        {
                            workSheet2.Cells[1, i] = getOnlineExcel(id).Columns[i - 1].ColumnName;
                            workSheet2.Cells.Font.Color = System.Drawing.Color.Black;
                        }
                        workSheet2.Cells[rowcount2, i] = datarow[i - 1].ToString();
                        if (rowcount2 > 3)
                        {
                            if (i == getOnlineExcel(id).Columns.Count)
                            {
                                cellRange2 = workSheet2.Range[workSheet2.Cells[rowcount2, 1], 
                                    workSheet2.Cells[rowcount2, getOnlineExcel(id).Columns.Count]];
                            }
                        }
                    }
                }

                cellRange2 = workSheet2.Range[workSheet2.Cells[1, 1], 
                    workSheet2.Cells[rowcount2, getOnlineExcel(id).Columns.Count]];
                cellRange2.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border2 = cellRange2.Borders;
                border2.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border2.Weight = 2d;
                cellRange2 = workSheet2.Range[workSheet2.Cells[1, 1], workSheet2.Cells[2, getOnlineExcel(id).Columns.Count]];

                workBook.SaveAs("~/Downloads/"+schedule.ScheduleName+ "_" +
                    schedule.Semester + "_" + schedule.AcademicYear + ".xls");
                workBook.Close();
                excel.Quit();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                workSheet = null;
                workSheet2 = null;
                cellRange = null;
                cellRange2 = null;
                workBook = null;
            }
            
            return View();
        }
        
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
                if (prevProf != i.Instructor)
                {
                    table.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "",
                        "", "", "", "", i.Instructor.HoursRequired);

                    foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId 
                        == i.Instructor.InstructorId))
                    {
                        if (row.DaysTaught != "ONL")
                        { 
                            table.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, "", 
                                row.StartTime.Value.TimeOfDay.ToString(), row.EndTime.Value.TimeOfDay.ToString(),
                                row.DaysTaught, row.Classroom.BldgCode + " " + row.Classroom.RoomNumber, row.numSeats,
                                row.Course.CreditHours, row.Classroom.Campus, "", "", "");
                        }
                    }
                    
                }
                    prevProf = i.Instructor;
               
            }

            return table;
        }

        public System.Data.DataTable getOnlineExcel(int? id)
        {
            Schedule schedule = db.Schedules.Find(id);
            var sections = db.Sections.Where(section => section.ScheduleId == id)
                .Where( section => section.DaysTaught == "ONL");
            
            System.Data.DataTable onlineTable = new System.Data.DataTable();
           
            onlineTable.Columns.Add("Instructor", typeof(string));
            onlineTable.Columns.Add("Course", typeof(string));
            onlineTable.Columns.Add("CRN", typeof(string));
            onlineTable.Columns.Add("ENRL", typeof(string));
            onlineTable.Columns.Add("AVAIL", typeof(string));
            onlineTable.Columns.Add("WLIST", typeof(string));
            onlineTable.Columns.Add("Pay", typeof(string));
            onlineTable.Columns.Add("Load/OVRL", typeof(string));
            onlineTable.Columns.Add(" ", typeof(string));

            Instructor prevProf = null;
            

            foreach(var i in schedule.sections.Where(section => section.DaysTaught == "ONL"))
            {

                onlineTable.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "", "");

                onlineTable.Rows.Add("", i.Course.Prefix + i.Course.CourseNumber, i.CRN, "",
                    i.Classroom.Capacity, "15", "", "", "");
            }
            
            return onlineTable;
        }
    }
}

