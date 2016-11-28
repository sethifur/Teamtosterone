using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Scheddy.ViewModels
{
    public class ClassroomByTime
    {
        public string BldgCode { get; set; }
        public string RoomNumber { get; set; }
        public string DaysTaught { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm tt}")]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm tt}")]
        public DateTime? EndTime { get; set; }
    }
}