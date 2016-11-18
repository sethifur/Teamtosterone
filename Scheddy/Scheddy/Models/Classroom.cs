using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string BldgCode { get; set; }
        [Required]
        public string RoomNumber { get; set; }
        [Required]
        public string Campus { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int NumComputers { get; set; }

        #endregion
    }
}