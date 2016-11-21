using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Scheddy.ViewModels
{
    public class ClassroomByTime
    {
                   public string BldgCode { get; set; }
                   public string RoomNumber { get; set; }
                   public DateTime StartTime { get; set; }
                   public DateTime EndTime { get; set; }
    }
}