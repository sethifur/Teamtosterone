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
    public class SectionController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        // GET: Section
        public ActionResult Index()
        {
            return View(db.Sections.ToList());
        }

        public ActionResult Create()
        {
            ClassroomCourseInstructorList list = new ClassroomCourseInstructorList();
            list.classrooms = db.Classrooms;
            list.courses = db.Courses;
            list.instructors = db.Instructors;
            
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassroomCourseInstructorList viewModel)
        {
            // ViewModels.ClassroomCourseInstructorList viewModel
            // [Bind(Include = "InstructorId,FirstName,LastName,HoursRequired,HoursReleased")] Instructor instructor

            String campus = viewModel.selectedClassroom.Split(' ')[0];
            String buildingCode = viewModel.selectedClassroom.Split(' ')[1];
            String roomNumber = viewModel.selectedClassroom.Split(' ')[2];

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);

            System.Diagnostics.Debug.WriteLine("HEY, GET READY");
            //System.Diagnostics.Debug.WriteLine(viewModel.classrooms.ToList()); this is null
            System.Diagnostics.Debug.WriteLine(campus + buildingCode + roomNumber + firstName + lastName + prefix + courseNumber);

            var chosenClassroom = from classroomFromDb in db.Classrooms
                                  where classroomFromDb.Campus == campus && classroomFromDb.BldgCode == buildingCode && classroomFromDb.RoomNumber == roomNumber
                                  select classroomFromDb;
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


            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.section.Course = db.Courses.Find(viewModel.section.CourseId);
                    viewModel.section.Classroom = db.Classrooms.Find(viewModel.section.ClassroomId);
                    viewModel.section.Instructor = db.Instructors.Find(viewModel.section.InstructorId);
                    viewModel.section.Schedule = db.Schedules.Find(viewModel.section.InstructorId);
                    try
                    {
                        db.Sections.Add(viewModel.section);
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine(ex.InnerException);
                        return RedirectToAction("Index");
                    }
                }
                catch (NullReferenceException e)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION assignments: " + e.Message);
                }
                
            }
            return View(viewModel.section);
        }

        public ActionResult Edit(int? id)
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
            return View(section);
        }


        // POST: Section/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstructorId,ClassroomId,StartDate,EndDate,EndTime,numSeats,DaysTaught")] Section section)
        {
            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(section);
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

            //remove section
            db.Sections.Remove(section);
            db.SaveChanges();
            return View(section);
        }

        public ActionResult GetSection(int? id)
        {
            //was an id passed in?
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //grab section to return
            Section section = db.Sections.Find(id);

            //does it exist?
            if (section == null)
            {
                return HttpNotFound();
            }

            //return it
            return View(section);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Section section = db.Sections.Find(id);
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Index");
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