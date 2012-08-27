using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class ReportedNews
    {
        #region Primitive properties

        public int Id { get; set; }
        public int NewsId { get; set; }
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
        [ForeignKey("NewsId")]
        public virtual News News { get; set; }

        #endregion
    }
}