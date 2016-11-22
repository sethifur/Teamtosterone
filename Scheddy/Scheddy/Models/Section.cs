using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Scheddy.Models
{
    public class Section
    {
        #region Relationships

        public virtual Course Course { get; set; }
        public int? CourseId { get; set; }

        public virtual Classroom Classroom { get; set; }
        public int? ClassroomId { get; set; }

        public virtual Instructor Instructor { get; set; }
        public int? InstructorId { get; set; }
        
        public virtual Schedule Schedule { get; set; }
        public int? ScheduleId { get; set; }

        #endregion

        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionId { get; set; }
        public int? CRN { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [DataType(DataType.Time)]
        public DateTime? StartTime { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string DaysTaught { get; set; }
        public string numSeats { get; set; }

        #endregion
    }
}