using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class ScheddyDb : DbContext
    {

        public ScheddyDb() : base("name=DefaultConnection")
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
    }
}