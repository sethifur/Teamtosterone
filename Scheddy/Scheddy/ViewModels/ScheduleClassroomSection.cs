using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheddy.Models;
using System.ComponentModel.DataAnnotations;

namespace Scheddy.ViewModels 
{
    public class ScheduleClassroomSection
    {
        //List of indexByProfessor objects
        public List<indexByClassroom> indexByClassroom { get; set; }

        //List of instructors
        public IEnumerable<Classroom> classroom { get; set; }

        public indexByClassroom index { get; set; }

        public int? scheduleId { get; set; }

        public ScheduleClassroomSection()
        {
            classroom = new List<Classroom>();
            indexByClassroom = new List<indexByClassroom>();
            scheduleId = 0;
        }
    }
}