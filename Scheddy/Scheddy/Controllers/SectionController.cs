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
using System.Web.Routing;
using Scheddy.ViewModels;

namespace Scheddy.Controllers
{
    public class SectionController : Controller
    {
        /// <summary>
        /// database connection
        /// </summary>
        ScheddyDb db = new ScheddyDb();


        /// <summary>
        /// default view for Sections. Don't know if we need this anymore.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(db.Sections.ToList());
        }


        /// <summary>
        /// returns Create view for adding a new section
        /// </summary>
        /// <param name="scheduleType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="classroom"></param>
        /// <param name="instructor"></param>
        /// <param name="daysTaught"></param>
        /// <returns></returns>
        public ActionResult Create(int scheduleType, DateTime? startTime, DateTime? endTime, string classroom = "", string instructor = "", string daysTaught = "")
        {
            ClassroomCourseInstructorList list = new ClassroomCourseInstructorList();
            list.classrooms = db.Classrooms;
            list.courses = db.Courses;
            list.instructors = db.Instructors;
            if (scheduleType > 0)
            {
                list.scheduleType = scheduleType;
            }
            if (startTime != null)
            {
                list.section.StartTime = startTime;
            }
            if (endTime != null)
            {
                list.section.EndTime = endTime;
            }
            if (!classroom.Equals(""))
            {
                list.selectedClassroom = classroom;
            }
            if (!instructor.Equals(""))
            {
                list.selectedInstructor = instructor;
            }
            if (!daysTaught.Equals(""))
            {
                list.section.DaysTaught = daysTaught;
            }

            return View(list);
        }


        /// <summary>
        /// POST method for adding a new section
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassroomCourseInstructorList viewModel, FormCollection f)
        {
            viewModel.section.ScheduleId = viewModel.scheduleId;

            String daysPerWeek = "";
            String campus = "";
            String buildingCode = "";
            String roomNumber = "";

            if (viewModel.checkedOnline)
            {
                daysPerWeek = "ONL";

                DateTime onlineBeginTime = DateTime.Parse("12/12/2016 12:00:00 AM");
                DateTime onlineEndTime = DateTime.Parse("12/12/2016 11:59:59 PM");

                viewModel.section.StartTime = onlineBeginTime;
                viewModel.section.EndTime = onlineEndTime;

                campus = "ONLINE";
                buildingCode = "OL";
                roomNumber = "ONLINE";

            }
            else
            {
                if (viewModel.checkedMonday)
                {
                    daysPerWeek += "M";
                }
                if (viewModel.checkedTuesday)
                {
                    daysPerWeek += "T";
                }
                if (viewModel.checkedWednesday)
                {
                    daysPerWeek += "W";
                }
                if (viewModel.checkedThursday)
                {
                    daysPerWeek += "R";
                }
                if (viewModel.checkedFriday)
                {
                    daysPerWeek += "F";
                }
                if (viewModel.checkedSaturday)
                {
                    daysPerWeek += "S";
                }

                campus = viewModel.selectedClassroom.Split(' ')[0] + " " + viewModel.selectedClassroom.Split(' ')[1];   //wonder if this works with ONLINE as campus
                buildingCode = viewModel.selectedClassroom.Split(' ')[2];
                roomNumber = viewModel.selectedClassroom.Split(' ')[3];
            }

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);

            var chosenClassroom = from classroom in db.Classrooms
                                  where
                                  classroom.Campus == campus && classroom.BldgCode == buildingCode && classroom.RoomNumber == roomNumber
                                  select classroom;

            try
            {
                viewModel.section.ClassroomId = chosenClassroom.First().ClassroomId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.ClassroomId: " + e.Message);
            }

            var chosenInstructor = from instructorFromDb in db.Instructors
                                   where instructorFromDb.FirstName == firstName && instructorFromDb.LastName == lastName
                                   select instructorFromDb;

            try
            {
                viewModel.section.InstructorId = chosenInstructor.First().InstructorId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.InstructorId: " + e.Message);
            }

            var chosenCourse = from courseFromDb in db.Courses
                               where courseFromDb.Prefix == prefix && courseFromDb.CourseNumber == courseNumberInt
                               select courseFromDb;
            try
            {
                viewModel.section.CourseId = chosenCourse.First().CourseId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.CourseId: " + e.Message);
            }

            try
            {
                viewModel.section.DaysTaught = daysPerWeek;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.DaysTaught: " + e.Message);
            }


            if (ModelState.IsValid)
            {

                try
                {
                    viewModel.section.Course = db.Courses.Find(viewModel.section.CourseId);
                    viewModel.section.Classroom = db.Classrooms.Find(viewModel.section.ClassroomId);
                    viewModel.section.Instructor = db.Instructors.Find(viewModel.section.InstructorId);
                    viewModel.section.Schedule = db.Schedules.Find(viewModel.section.ScheduleId);

                    string conflict = CheckConflict(viewModel.scheduleId, viewModel.section);

                    if (!conflict.Equals("")) //  There was a conflict. Return to the view and present a validation error.
                    {
                        ModelState.AddModelError("", conflict);

                        viewModel.classrooms = db.Classrooms;
                        viewModel.courses = db.Courses;
                        viewModel.instructors = db.Instructors;

                        return View(viewModel);
                    }
                    else
                    {   // we're golden. Attempt to add.
                        try
                        {
                            // show alert if overtime hours
                            int hoursWorking = 0;
                            foreach (Section section in viewModel.section.Instructor.sections) // add courses already being taught
                            {
                                hoursWorking += section.Course.CreditHours;
                            }
                            hoursWorking += viewModel.section.Course.CreditHours; // add course about to be added
                            System.Diagnostics.Debug.WriteLine("hoursWorking:" + hoursWorking);
                            if (hoursWorking > viewModel.section.Instructor.HoursRequired)
                            {
                                string msg = "Instructor " + viewModel.section.Instructor.FirstName + " " + viewModel.section.Instructor.LastName + " now working overtime with " + hoursWorking + " hours.";
                                PrintAlert(msg);
                            }
                            
                            db.Sections.Add(viewModel.section);
                            db.SaveChanges();
                        }
                        catch (DbUpdateException ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.InnerException);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION assignments: " + e.Message);
                }
            }

            if (viewModel.scheduleType == 1)
            {
                return new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action", "IndexByClassroom/"+viewModel.section.ScheduleId},
                    {"controller", "Schedule"}
                }
                );
            }
            else if (viewModel.scheduleType == 2)
            {
                return new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action", "IndexByProfessor/"+viewModel.section.ScheduleId},
                    {"controller", "Schedule"}
                }
                );
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        protected void PrintAlert(string msg) { Response.Write("<script>alert('" + msg + "')</script>"); }


        /// <summary>
        /// Edit Section. This most likely needs work
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheduleType"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id, int scheduleType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }

            ClassroomCourseInstructorList list = new ClassroomCourseInstructorList();

            list.classrooms = db.Classrooms;
            list.courses = db.Courses;
            list.instructors = db.Instructors;

            list.selectedInstructor = section.Instructor.FirstName + " " + section.Instructor.LastName;
            list.selectedCourse = section.Course.Prefix + " " + section.Course.CourseNumber;
            list.selectedClassroom = section.Classroom.Campus + " " + section.Classroom.BldgCode + " " + section.Classroom.RoomNumber;

            list.section = section;

            list.scheduleType = scheduleType;

            if (list.section.DaysTaught == "ONL")
            {
                list.selectedClassroom = "ONLINE";
            }

            return View(list);
        }


        /// <summary>
        /// POST: Section/Edit/5
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassroomCourseInstructorList viewModel)
        {
            String daysPerWeek = "";
            String campus = "";
            String buildingCode = "";
            String roomNumber = "";

            if (viewModel.checkedOnline)
            {
                daysPerWeek = "ONL";

                DateTime onlineBeginTime = DateTime.Parse("12/12/2016 12:00:00 AM");
                DateTime onlineEndTime = DateTime.Parse("12/12/2016 11:59:59 PM");

                viewModel.section.StartTime = onlineBeginTime;
                viewModel.section.EndTime = onlineEndTime;

                campus = "ONLINE";
                buildingCode = "OL";
                roomNumber = "ONLINE";
            }
            else
            {
                if (viewModel.checkedMonday)
                {
                    daysPerWeek += "M";
                }
                if (viewModel.checkedTuesday)
                {
                    daysPerWeek += "T";
                }
                if (viewModel.checkedWednesday)
                {
                    daysPerWeek += "W";
                }
                if (viewModel.checkedThursday)
                {
                    daysPerWeek += "R";
                }
                if (viewModel.checkedFriday)
                {
                    daysPerWeek += "F";
                }
                if (viewModel.checkedSaturday)
                {
                    daysPerWeek += "S";
                }

                campus = viewModel.selectedClassroom.Split(' ')[0] + " " + viewModel.selectedClassroom.Split(' ')[1];
                buildingCode = viewModel.selectedClassroom.Split(' ')[2];
                roomNumber = viewModel.selectedClassroom.Split(' ')[3];
            }

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);

            var chosenClassroom = from classroom in db.Classrooms
                                  where
                                  classroom.Campus == campus && classroom.BldgCode == buildingCode && classroom.RoomNumber == roomNumber
                                  select classroom;

            try
            {
                viewModel.section.ClassroomId = chosenClassroom.First().ClassroomId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.ClassroomId: " + e.Message);
            }

            var chosenInstructor = from instructorFromDb in db.Instructors
                                   where instructorFromDb.FirstName == firstName && instructorFromDb.LastName == lastName
                                   select instructorFromDb;
            try
            {
                viewModel.section.InstructorId = chosenInstructor.First().InstructorId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.InstructorId: " + e.Message);
            }

            var chosenCourse = from courseFromDb in db.Courses
                               where courseFromDb.Prefix == prefix && courseFromDb.CourseNumber == courseNumberInt
                               select courseFromDb;
            try
            {
                viewModel.section.CourseId = chosenCourse.First().CourseId;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.CourseId: " + e.Message);
            }

            try
            {
                viewModel.section.DaysTaught = daysPerWeek;
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION viewModel.section.DaysTaught: " + e.Message);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.section.Course = db.Courses.Find(viewModel.section.CourseId);
                    viewModel.section.Classroom = db.Classrooms.Find(viewModel.section.ClassroomId);
                    viewModel.section.Instructor = db.Instructors.Find(viewModel.section.InstructorId);
                    viewModel.section.Schedule = db.Schedules.Find(viewModel.section.ScheduleId);

                    string conflict = CheckConflict(viewModel.section.ScheduleId, viewModel.section); // forcing 1 as schedule id for now. Need to update this in the future.

                    if (!conflict.Equals("")) //  There was a conflict. Return to the view and present a validation error.
                    {
                        ModelState.AddModelError("", conflict);

                        viewModel.classrooms = db.Classrooms;
                        viewModel.courses = db.Courses;
                        viewModel.instructors = db.Instructors;

                        return View(viewModel);

                    }
                    else
                    {   // we're golden. Attempt to edit.
                        try
                        {
                            Section section = db.Sections.Find(viewModel.section.SectionId);
                            db.Sections.Remove(section);
                            db.SaveChanges();
                            db.Sections.Add(viewModel.section);
                            db.SaveChanges();

                            if (viewModel.scheduleType == 1)
                            {
                                return new RedirectToRouteResult(new RouteValueDictionary
                                {
                                    {"action", "IndexByClassroom/"+viewModel.section.ScheduleId},
                                    {"controller", "Schedule"}
                                });
                            }
                            else if (viewModel.scheduleType == 2)
                            {
                                return new RedirectToRouteResult(new RouteValueDictionary
                                {
                                    {"action", "IndexByProfessor/"+viewModel.section.ScheduleId},
                                    {"controller", "Schedule"}
                                });
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        catch (DbUpdateException ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.InnerException);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION assignments: " + e.Message);
                }
            }

            viewModel.classrooms = db.Classrooms;
            viewModel.courses = db.Courses;
            viewModel.instructors = db.Instructors;

            return View(viewModel);
        }


        /// <summary>
        /// Returns view to delete a section
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheduleType"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id, int scheduleType)
        {
            //was an id passed in?
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //grab section to delete
            Section section = db.Sections.Find(id);

            //does section exist?
            if (section == null)
            {
                return HttpNotFound();
            }

            SectionScheduleType viewModel = new SectionScheduleType();
            viewModel.section = section;
            viewModel.scheduleType = scheduleType;

            return View(viewModel);
        }


        /// <summary>
        /// POST: Instructor/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, SectionScheduleType viewModel)
        {
            Section section = db.Sections.Find(id);
            int scheduleId = (int) section.ScheduleId;

            try
            {
                db.Sections.Remove(section);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(" HERE HRE HEKH ERKH ERKHER : " + e.Message);
                return RedirectToAction("CannotDelete", viewModel);
            }

            if (viewModel.scheduleType == 1)
            {
                return new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action", "IndexByClassroom/"+scheduleId},
                    {"controller", "Schedule"}
                });
            }
            else if (viewModel.scheduleType == 2)
            {
                return new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action", "IndexByProfessor/"+scheduleId},
                    {"controller", "Schedule"}
                });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// returns view when section cannot be deleted
        /// </summary>
        /// <returns></returns>
        public ActionResult CannotDelete(SectionScheduleType viewModel)
        {
            return View(viewModel);
        }

        
        /// <summary>
        /// checks for conflicts in the existing schedule
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="newSection"></param>
        /// <returns></returns>
        public string CheckConflict(int? scheduleId, Section newSection)
        {
            Schedule schedule = db.Schedules.Find(scheduleId);

            string conflictMessage = "";

            if (schedule.sections.Count == 0)
            {
                return conflictMessage;
            }

            var newJustStartTime = newSection.StartTime.Value.TimeOfDay;
            var newJustEndTime = newSection.EndTime.Value.TimeOfDay;

            foreach (Section section in schedule.sections)
            {
                if (section.SectionId != newSection.SectionId)
                {
                    if (section.CRN == newSection.CRN)
                    {
                        return "Course with that CRN already exists.";
                    }
                    var justStartTime = section.StartTime.Value.TimeOfDay;
                    var justEndTime = section.EndTime.Value.TimeOfDay;

                    if (commonDay(section.DaysTaught, newSection.DaysTaught)) // two sections share a common day
                    {
                        if (justStartTime < newJustEndTime && newJustStartTime < justEndTime) // times overlap
                        {
                            string newConflictMessage = "";

                            // instructors at same time conflict
                            if (section.Instructor == newSection.Instructor)
                            {
                                newConflictMessage += ", Instructor teaching another class";
                            }

                            // course has another section at same time conflict
                            if (section.Course == newSection.Course)
                            {
                                newConflictMessage += ", Course taught at same time";
                            }

                            // classrooms at same time conflict
                            if (section.Classroom == newSection.Classroom)
                            {
                                newConflictMessage += ", Classroom already in use";
                            }

                            if (!newConflictMessage.Equals(""))
                            {
                                newConflictMessage = " CONFLICT with: ("
                                    + section.Classroom.Campus + " " + section.Classroom.BldgCode + " " + section.Classroom.RoomNumber + ", "
                                    + section.Instructor.FirstName + " " + section.Instructor.LastName + ", "
                                    + section.Course.Prefix + " " + section.Course.CourseNumber + ", "
                                    + section.DaysTaught + " " + section.StartTime.Value.TimeOfDay + " - " + section.EndTime.Value.TimeOfDay + ")"
                                    + newConflictMessage;

                                conflictMessage += newConflictMessage;
                            }
                        }
                    }
                }
            }

            // class not starting on half hour conflict
            if (newSection.StartTime.Value.TimeOfDay.Minutes != 30 && newSection.DaysTaught != "ONL")
            {
                conflictMessage += " STARTTIME CONFLICT: StartTime must begin on the half hour";
            }

            return conflictMessage;
        }


        /// <summary>
        /// common day method. used in checking for conflicts
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool commonDay(string first, string second)
        {
            if (first.Contains("ONL") || second.Contains("ONL")) // TODO: Change "ONL" to const
            {
                return false;
            }

            int numOfCommonDays = first.GroupBy(c => c)
                .Join(
                second.GroupBy(c => c),
                g => g.Key,
                g => g.Key,
                (lg, rg) => lg.Zip(rg, (l, r) => l).Count())
                .Sum();

            if (numOfCommonDays > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// deletes database connection
        /// </summary>
        /// <param name="disposing"></param>
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