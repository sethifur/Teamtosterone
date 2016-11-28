using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheddy.Models;
using System.ComponentModel.DataAnnotations;

namespace Scheddy.ViewModels
{
    public class ScheduleInstructorSection
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BldgCode { get; set; }
        public string RoomNumber { get; set; }
        public string DaysTaught { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime? EndTime { get; set; }

        public IEnumerable<Section> sections { get; set; }
        //public ScheduleInstructorSection()
        //{
        //    List<Section> sections  = new List<Section>();
        //}

    }
}