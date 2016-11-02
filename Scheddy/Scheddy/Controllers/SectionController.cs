using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Scheddy.Controllers
{
    public class SectionController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Section
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateSection()
        {


        }
        public void DeleteSection()
        {

        }
        public void AddSection()
        {

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
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}