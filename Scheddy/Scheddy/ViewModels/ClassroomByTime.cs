using Scheddy.Models;
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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [DataType(DataType.Time)]
        public DateTime? StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        public IEnumerable<Section> sections { get; set; }
    }
}