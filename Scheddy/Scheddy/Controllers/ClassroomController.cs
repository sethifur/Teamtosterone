using Scheddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheddy.Controllers
{
    public class ClassroomController : Controller
    {
        // GET: Classroom
        public ActionResult Index()
        {
            return View();
        }

        public void UpdateClassroom()
        {


        }

        public void DeleteClassroom()
        {

        }
        
        public void AddClassroom()
        {

        }

        public Classroom GetClassroom()
        {

            return new Classroom();
        }

        public void SetClassroom()
        {

        }
    }
}