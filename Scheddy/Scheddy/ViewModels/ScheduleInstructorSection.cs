using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheddy.ViewModels
{
    public class ScheduleInstructorSection
    {
        public IEnumerable<Models.Instructor> instructor;
        public IEnumerable<Models.Section> section;
        public Models.Schedule schedule;

        public ScheduleInstructorSection ()
        {
            List<string> instructor = new List<string>();
            List<string> section = new List<string>();
        }
          
        
    }
}