﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Schedule
    {
        #region Relationships

        public virtual ICollection<Section> sections { get; set; }

        #endregion

        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleId { get; set; }
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public string ScheduleName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        #endregion
    }
}