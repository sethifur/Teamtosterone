using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheddy.Models;

namespace Scheddy.ViewModels
{
    public class SectionViewModel
    {
        public int? CourseId { get; set; }
        public int? ClassroomId { get; set; }
        public int? InstructorId { get; set; }
        public int? ScheduleId { get; set; }
        public int? CRN { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DaysTaught { get; set; }
        public string numSeats { get; set; }
    }
}