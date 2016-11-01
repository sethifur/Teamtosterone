using Scheddy.Models;
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

<<<<<<< HEAD
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

=======
        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
>>>>>>> fa75e824be5cc41c9f80c12429d1acd4f1eb90c4
        }
    }
}