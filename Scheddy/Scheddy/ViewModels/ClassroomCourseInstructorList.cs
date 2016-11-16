﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheddy.ViewModels
{
    public class ClassroomCourseInstructorList
    {
        public IEnumerable<Models.Classroom> classrooms;
        public IEnumerable<Models.Course> courses;
        public IEnumerable<Models.Instructor> instructors;
        public Models.Section section;

        public string selectedClassroom { get; set; }
        public string selectedCourse { get; set; }
        public string selectedInstructor { get; set; }

    }
}