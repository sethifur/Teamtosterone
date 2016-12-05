using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scheddy.ViewModels
{
    public class indexByClassroom
    {

        public string BldgCode { get; set; }
        public string RoomNumber { get; set; }
        public string DaysTaught { get; set; }
        public string Campus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Prefix { get; set; }
        public int CourseNumber { get; set; }
        public int SectionId { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime? EndTime { get; set; }

    }
}