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
        ScheddyDb db = new ScheddyDb();

        // GET: Section
        public ActionResult Index()
        {
            return View(db.Sections.ToList());
        }
        
        /*
        public ActionResult Create()
        {
            ClassroomCourseInstructorList list = new ClassroomCourseInstructorList();
            list.classrooms = db.Classrooms;
            list.courses = db.Courses;
            list.instructors = db.Instructors;

            return View(list);
        }
        */

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassroomCourseInstructorList viewModel, FormCollection f)
        {
            // ViewModels.ClassroomCourseInstructorList viewModel
            // [Bind(Include = "InstructorId,FirstName,LastName,HoursRequired,HoursReleased")] Instructor instructor

            // TODO: Not have this assigned this way.
            viewModel.section.ScheduleId = 1;

            String daysPerWeek = "";
            if (viewModel.checkedOnline)
            {
                daysPerWeek = "ONL";
            } else
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
            }


            String campus = viewModel.selectedClassroom.Split(' ')[0] + " " + viewModel.selectedClassroom.Split(' ')[1];
            String buildingCode = viewModel.selectedClassroom.Split(' ')[2];
            String roomNumber = viewModel.selectedClassroom.Split(' ')[3];

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);

            //System.Diagnostics.Debug.WriteLine("HEY, GET READY");
            //System.Diagnostics.Debug.WriteLine(viewModel.classrooms.ToList()); this is null
            //System.Diagnostics.Debug.WriteLine(campus + buildingCode + roomNumber + firstName + lastName + prefix + courseNumber);

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

                    string conflict = CheckConflict(1, viewModel.section); // forcing 1 as schedule id for now. Need to update this in the future.

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
                    {"action", "IndexByClassroom"},
                    {"controller", "Schedule"}
                }
                );
            }
            else if (viewModel.scheduleType == 2)
            {
                return new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action", "IndexByProfessor"},
                    {"controller", "Schedule"}
                }
                );
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

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

            return View(list);
            
        }


        // POST: Section/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassroomCourseInstructorList viewModel)
        {
            String daysPerWeek = "";
            if (viewModel.checkedOnline)
            {
                daysPerWeek = "ONL";
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
            }


            String campus = viewModel.selectedClassroom.Split(' ')[0] + " " + viewModel.selectedClassroom.Split(' ')[1];
            String buildingCode = viewModel.selectedClassroom.Split(' ')[2];
            String roomNumber = viewModel.selectedClassroom.Split(' ')[3];

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);

            //System.Diagnostics.Debug.WriteLine("HEY, GET READY");
            //System.Diagnostics.Debug.WriteLine(viewModel.classrooms.ToList()); this is null
            //System.Diagnostics.Debug.WriteLine(campus + buildingCode + roomNumber + firstName + lastName + prefix + courseNumber);

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

                    string conflict = CheckConflict(1, viewModel.section); // forcing 1 as schedule id for now. Need to update this in the future.

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
                                    {"action", "IndexByClassroom"},
                                    {"controller", "Schedule"}
                                }
                                );
                            }
                            else if (viewModel.scheduleType == 2)
                            {
                                return new RedirectToRouteResult(new RouteValueDictionary
                                {
                                    {"action", "IndexByProfessor"},
                                    {"controller", "Schedule"}
                                }
                                );
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

        public ActionResult Delete(int? id)
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
            
            return View(section);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Section section = db.Sections.Find(id);
            try
            {
                db.Sections.Remove(section);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return RedirectToAction("CannotDelete");
            }

            return RedirectToAction("Index");
        }

        public ActionResult CannotDelete()
        {
            return View();
        }

        //this should probably be a Javascript function on the view when it's made.
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

            // intructor overworking conflict
            int hoursWorking = 0;
            foreach (Section section in newSection.Instructor.sections)
            {
                hoursWorking += section.Course.CreditHours;
            }
            if (hoursWorking > newSection.Instructor.HoursRequired)
            {
                conflictMessage += " HOURS CONFLICT: Instructor at max hours ("
                                + newSection.Instructor.FirstName + " " + newSection.Instructor.LastName + ", "
                                + hoursWorking + " credit hours)";
            }

            return conflictMessage;
        }

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