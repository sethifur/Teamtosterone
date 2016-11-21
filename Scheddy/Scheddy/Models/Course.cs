using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Course
    {
        #region Relationships

        public virtual ICollection<Section> sections { get; set; }

        #endregion

        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }
        [Required]
        public int CourseNumber { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Prefix { get; set; }
        [Required]
        public int CreditHours { get; set; }
        [Required]
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }

        #endregion
    }
}