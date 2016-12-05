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
            ScheduleInstructorSection model = new ScheduleInstructorSection();

           //for (int i = 0; i < db.Sections.Count(); i++)
            //{
                var query = from ii in db.Instructors
                            join s in db.Sections on
                            ii.InstructorId equals s.InstructorId
                            join c in db.Classrooms on
                            s.ClassroomId equals c.ClassroomId
                            orderby s.StartTime ascending
                            select new
                            { ii, s, c };
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
                        EndTime = item.s.EndTime
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
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook worKbooK;
            Microsoft.Office.Interop.Excel.Workbook WorkBook;
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
            Microsoft.Office.Interop.Excel.Worksheet WorkSheet;
            Microsoft.Office.Interop.Excel.Range celLrangE;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;
                excel.DisplayAlerts = false;
                worKbooK = excel.Workbooks.Add(Type.Missing);
                WorkBook = excel.Workbooks.Add(Type.Missing);

                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                worKsheeT.Name = "CS FTF_"+schedule.Semester+"_"+schedule.AcademicYear;

                worKsheeT.Cells.Font.Size = 12;

                int rowcount = 1;

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
                

                WorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)WorkBook.ActiveSheet;
                WorkSheet.Name = "CS Onl_" + schedule.Semester + "_" + schedule.AcademicYear;

                WorkSheet.Cells.Font.Size = 12;

                rowcount = 1;
                foreach (DataRow datarow in getOnlineExcel(id).Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= getOnlineExcel(id).Columns.Count; i++)
                    {

                        if (rowcount == 3)
                        {
                            WorkSheet.Cells[2, i] = getOnlineExcel(id).Columns[i - 1].ColumnName;
                            WorkSheet.Cells.Font.Color = System.Drawing.Color.Black;

                        }

                        WorkSheet.Cells[rowcount, i] = datarow[i - 1].ToString();

                        if (rowcount > 3)
                        {
                            if (i == getOnlineExcel(id).Columns.Count)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    celLrangE = worKsheeT.Range[WorkSheet.Cells[rowcount, 1], WorkSheet.Cells[rowcount, getOnlineExcel(id).Columns.Count]];
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
                //WorkBook.SaveAs("~/Downloads/" + schedule.ScheduleName + "_" + schedule.Semester + "_" + schedule.AcademicYear + ".xls");
                worKbooK.SaveAs("~/Downloads/"+schedule.ScheduleName+ "_" + schedule.Semester + "_" + schedule.AcademicYear + ".xls");
                WorkBook.Close();
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
                WorkSheet = null;
                celLrangE = null;
                worKbooK = null;
                WorkBook = null;
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
                int hoursWorking = 0;
                foreach (Section section in schedule.sections)
                {
                    hoursWorking += section.Course.CreditHours;
                }

                if (prevProf != i.Instructor)
                {

                    hoursWorking += i.Instructor.HoursReleased;
                    table.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "",
                        "", "", "", "", i.Instructor.HoursRequired);

                    foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == i.Instructor.InstructorId))
                    {
                        if (row.DaysTaught != "ONL")
                        {
                            table.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, "", row.StartTime.Value.TimeOfDay.ToString(),
                                row.EndTime.Value.TimeOfDay.ToString(), row.DaysTaught, row.Classroom.BldgCode + " " + row.Classroom.RoomNumber,
                                row.numSeats, row.Course.CreditHours, row.Classroom.Campus, "", "", "");
                        }
                    }
                    
                }
                    prevProf = i.Instructor;
               
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
            onlineTable.Columns.Add(" ", typeof(string));

            Instructor prevProf = null;
            foreach (var i in schedule.sections)
            {
                int hoursWorking = 0;
                foreach (Section section in schedule.sections)
                {
                    hoursWorking += section.Course.CreditHours;
                }

                if (prevProf != i.Instructor)
                {
                    hoursWorking += i.Instructor.HoursReleased;
                    onlineTable.Rows.Add(i.Instructor.LastName + ", " + i.Instructor.FirstName, "", "", "", "", "", "", "", "");

                    foreach (var row in schedule.sections.Where(section => section.Instructor.InstructorId == i.Instructor.InstructorId))
                    {
                        if (row.DaysTaught == "ONL")
                        {
                            onlineTable.Rows.Add("", row.Course.Prefix + row.Course.CourseNumber, row.CRN, "",
                                row.Classroom.Capacity, "", "", "", "");
                        }

                    }
                    
                }
                    prevProf = i.Instructor;     
            }
            
            return onlineTable;
        }
    }
}

