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
    }
}