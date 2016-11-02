using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Scheddy.Models
{
    public class Classroom
    {
        #region Relationships

        public virtual ICollection<Section> sections { get; set; }

        #endregion

        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ClassroomId { get; set; }
        public string BldgCode { get; set; }
        public string RoomNumber { get; set; }
        public string Campus { get; set; }
        public int Capacity { get; set; }
        public int NumComputers { get; set; }

        #endregion
    }
}