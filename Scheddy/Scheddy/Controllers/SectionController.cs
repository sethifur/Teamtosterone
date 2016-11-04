using Scheddy.Models;
using System;
using System.Collections.Generic;
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

        public ActionResult UpdateSection(int id, SectionViewModel sectionVm)
        {
            Section section = new Section();
            
            //set section object to View Model data        
            
            db.Sections.Add(section);
            return View(section);
        }

        public ActionResult DeleteSection(int? id)
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

        public ActionResult AddSection(SectionViewModel sectionVm)
        {
            return View();
        }

        public Section GetSection()
        {

            return new Section();
        }

        public void SetSection()
        {

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