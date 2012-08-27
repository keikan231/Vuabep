using System;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class Word
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        [StringLengthExtended(20)]
        [Display(Name = "Word_Key", ResourceType = typeof(Labels))]
        public string Key { get; set; }
        [RequiredExtended]
        [StringLengthExtended(100)]
        [Display(Name = "Word_Value", ResourceType = typeof(Labels))]
        public string Value { get; set; }
        [Column("CreatedBy")]
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [Column("ApprovedBy")]
        public int? ApprovedById { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }

        [ForeignKey("ApprovedById")]
        public virtual User ApprovedBy { get; set; }

        #endregion
    }
}