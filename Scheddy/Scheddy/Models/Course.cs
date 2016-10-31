using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Course
    {
        public int CourseNumber { get; set; }
        public string Prefix { get; set; }
        public int CreditHours { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
    }
}