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
        public ActionResult Create(ClassroomCourseInstructorList viewModel, FormCollection f)
        {
            // ViewModels.ClassroomCourseInstructorList viewModel
            // [Bind(Include = "InstructorId,FirstName,LastName,HoursRequired,HoursReleased")] Instructor instructor

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

            System.Diagnostics.Debug.WriteLine("HERE 1");

            if (ModelState.IsValid)
            {

                System.Diagnostics.Debug.WriteLine("HERE 2");
                try
                {
                    System.Diagnostics.Debug.WriteLine("HERE 3");

                    viewModel.section.Course = db.Courses.Find(viewModel.section.CourseId);
                    viewModel.section.Classroom = db.Classrooms.Find(viewModel.section.ClassroomId);
                    viewModel.section.Instructor = db.Instructors.Find(viewModel.section.InstructorId);
                    viewModel.section.Schedule = db.Schedules.Find(viewModel.section.InstructorId);
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("HERE 4");
                        db.Sections.Add(viewModel.section);
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("HERE OH NO 1");
                        System.Diagnostics.Debug.WriteLine(ex.InnerException);
                        Console.WriteLine(ex.InnerException);
                        return RedirectToAction("Index");
                    }
                }
                catch (NullReferenceException e)
                {
                    System.Diagnostics.Debug.WriteLine("HERE OH NO 2");
                    System.Diagnostics.Debug.WriteLine("EXCEPTION assignments: " + e.Message);
                }

            }
            return RedirectToAction("Index");
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
        public ActionResult Edit(
            [Bind(Include = "InstructorId,ClassroomId,StartDate,EndDate,EndTime,numSeats,DaysTaught")] Section section)
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