﻿using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Scheddy.ViewModels;


namespace Scheddy.Controllers
{
    public class SectionController : Controller
    {
        ScheddyDb db = new ScheddyDb();

        // GET: Section
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstructorId,ClassroomId,StartDate,EndDate,EndTime,numSeats,DaysTaught")] Section section)
        {
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(section);

        
        }
            
        public ActionResult Update(int? id, SectionViewModel sectionVm)
        {
            //was in id passed in?
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //grab section to update
            Section section = db.Sections.Find(id);

            //does that section exist?
            if (section == null)
            {
                return HttpNotFound();
            }

            //set section object to View Model data
            section.CourseId = sectionVm.CourseId;
            section.InstructorId = sectionVm.InstructorId;
            section.ClassroomId = sectionVm.ClassroomId;
            section.StartDate = sectionVm.StartDate;
            section.EndDate = sectionVm.EndDate;
            section.StartTime = sectionVm.StartTime;
            section.EndTime = sectionVm.EndTime;
            section.numSeats = sectionVm.numSeats;
            section.DaysTaught = sectionVm.DaysTaught;

            //update section
            db.Sections.AddOrUpdate(section);
            db.SaveChanges();
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

        public void SetSection()
        {
            //?? I'm not sure what this method is compared to AddSection()
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