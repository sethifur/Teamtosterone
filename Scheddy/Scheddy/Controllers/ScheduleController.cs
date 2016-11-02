using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Scheddy.Controllers
{
    public class ScheduleController : Controller
    {
        ScheddyDb _db = new ScheddyDb();

        // GET: Schedule
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateSchedule()
        {

        }

        public void DeleteSchedule()
        {

        }

        public void AddSchedule()
        {

        }
    
        public Section CheckConflict()
        {

            return new Section();
        }

        public Schedule GetSchedule()
        {

            return new Schedule();
        }

        public void SetSchedule()
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