using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Instructor
    {
        #region Relationships

        public virtual ICollection<Section> sections { get; set; }

        #endregion

        #region Properties

        public int InstructorId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName{ get; set; }
        [Required]
        public int HoursRequired { get; set; }
        [Required]
        public int HoursReleased { get; set; }

        #endregion
    }
}