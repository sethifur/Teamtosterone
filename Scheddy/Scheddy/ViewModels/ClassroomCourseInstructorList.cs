using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheddy.Models;


namespace Scheddy.ViewModels
{
    public class ClassroomCourseInstructorList
    {
        public IEnumerable<Classroom> classrooms { get; set; }
        public IEnumerable<Course> courses { get; set; }
        public IEnumerable<Instructor> instructors { get; set; }
        public Section section { get; set; }

        public string selectedClassroom { get; set; }
        public string selectedCourse { get; set; }
        public string selectedInstructor { get; set; }

        public ClassroomCourseInstructorList()
        {
            section = new Section();
        }
    }
}