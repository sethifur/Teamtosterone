﻿using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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
            ViewModels.ClassroomCourseInstructorList list = new ViewModels.ClassroomCourseInstructorList();
            list.classrooms = db.Classrooms;
            list.courses = db.Courses;
            list.instructors = db.Instructors;
            
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewModels.ClassroomCourseInstructorList viewModel)
        {
            String campus = viewModel.selectedClassroom.Split(' ')[0];
            String buildingCode = viewModel.selectedClassroom.Split(' ')[1];
            String roomNumber = viewModel.selectedClassroom.Split(' ')[2];

            String firstName = viewModel.selectedInstructor.Split(' ')[0];
            String lastName = viewModel.selectedInstructor.Split(' ')[1];

            String prefix = viewModel.selectedCourse.Split(' ')[0];
            String courseNumber = viewModel.selectedCourse.Split(' ')[1];
            int courseNumberInt = Int32.Parse(courseNumber);


            var chosenClassroom = from classroomFromDb in db.Classrooms
                      where classroomFromDb.Campus == campus && classroomFromDb.BldgCode == buildingCode && classroomFromDb.RoomNumber == roomNumber
                      select classroomFromDb;
            viewModel.section.ClassroomId = chosenClassroom.First().ClassroomId;

            var chosenInstructor = from instructorFromDb in db.Instructors
                                  where instructorFromDb.FirstName == firstName && instructorFromDb.LastName == lastName
                                  select instructorFromDb;
            viewModel.section.InstructorId = chosenInstructor.First().InstructorId;

            var chosenCourse = from courseFromDb in db.Courses
                                   where courseFromDb.Prefix == prefix && courseFromDb.CourseNumber == courseNumberInt
                               select courseFromDb;
            viewModel.section.InstructorId = chosenInstructor.First().InstructorId;


            if (ModelState.IsValid)
            {
                viewModel.section.Course = db.Courses.Find(viewModel.section.CourseId);
                viewModel.section.Classroom = db.Classrooms.Find(viewModel.section.ClassroomId);
                viewModel.section.Instructor = db.Instructors.Find(viewModel.section.InstructorId);
                viewModel.section.Schedule = db.Schedules.Find(viewModel.section.ScheduleId);
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