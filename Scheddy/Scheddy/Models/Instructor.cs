using System;
using System.Collections.Generic;
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
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public int HoursRequired { get; set; }
        public int HoursReleased { get; set; }

        #endregion
    }
}