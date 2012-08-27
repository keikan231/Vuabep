using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class ReportedTip
    {
        #region Primitive properties

        public int Id { get; set; }
        public int TipId { get; set; }
        [Column("ReportedBy")]
        public int ReportedById { get; set; }
        public DateTime ReportedDate { get; set; }
        [StringLength(100)]
        public String Reason { get; set; }
        public bool IsIgnored { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("ReportedById")]
        public virtual User ReportedBy { get; set; }
        [ForeignKey("TipId")]
        public virtual Tip Tip { get; set; }

        #endregion
    }
}