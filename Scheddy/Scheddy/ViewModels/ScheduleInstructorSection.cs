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
        //List of indexByProfessor objects
        public List<indexByProfessor> indexByProfessor { get; set; }

        //List of instructors
        public IEnumerable<Instructor> instructor { get; set; }

        public indexByProfessor index { get; set; }

        public int? scheduleId { get; set; }

        public ScheduleInstructorSection ()
        {
            instructor = new List<Instructor>();
            indexByProfessor = new List<indexByProfessor>();
        }

       
    }
}