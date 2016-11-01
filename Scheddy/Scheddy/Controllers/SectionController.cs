using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Scheddy.Models;

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