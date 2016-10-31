using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Schedule
    {
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public string ScheduleName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}