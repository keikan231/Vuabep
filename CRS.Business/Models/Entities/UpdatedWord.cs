using System;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class UpdatedWord
    {
        #region Primitive properties

        public int Id { get; set; }
        public int WordId { get; set; }
        [RequiredExtended]
        [StringLengthExtended(100)]
        [Display(Name = "Word_Value", ResourceType = typeof(Labels))]
        public string NewValue { get; set; }
        [Column("UpdatedBy")]
        public int UpdatedById { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsApproved { get; set; }
        [Column("ApprovedBy")]
        public int? ApprovedById { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("ApprovedById")]
        public virtual User ApprovedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User UpdatedBy { get; set; }

        [ForeignKey("WordId")]
        public virtual Word Words { get; set; }

        #endregion
    }
}