using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheddy.Controllers
{
    public class InstructorController : Controller
    {
        // GET: Instructor
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateInstructor()
        {


        }

        public void DeleteInstructor()
        {

        }

        public void AddInstructor()
        {

        }

        public Instructor GetInstructor()
        {

            return new Instructor();
        }

        public void SetInstructor()
        {

        }
    }
}