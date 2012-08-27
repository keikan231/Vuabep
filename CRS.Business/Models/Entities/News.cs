using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class News
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        [StringLengthExtended(100, MinimumLength = 10)]
        [Display(Name = "News_Title", ResourceType = typeof(Labels))]
        public string Title { get; set; }
        public string TitleSearch { get; set; }
        [StringLengthExtended(1000)]
        public string TitleUrl { get; set; }
        [StringLengthExtended(1000)]
        [Display(Name = "News_Image", ResourceType = typeof(Labels))]
        public string ImageUrl { get; set; }
        [RequiredExtended]
        [StringLengthExtended(255, MinimumLength = 10)]
        [Display(Name = "News_ShortDescription", ResourceType = typeof(Labels))]
        public string ContentText { get; set; }
        [RequiredExtended]
        [StringLengthExtended(int.MaxValue / 2)] // Don't remove this line otherwise EF will throw a validation exception
        [Display(Name = "News_Description", ResourceType = typeof(Labels))]
        [AllowHtml]
        public string ContentHtml { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Reports { get; set; }

        [Column("PostedBy")]
        public int PostedById { get; set; }

        public DateTime PostedDate { get; set; }

        [Column("UpdatedBy")]
        public int? UpdatedById { get; set; }
        public DateTime? LastUpdate { get; set; }

        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties
        [ForeignKey("PostedById")]
        public virtual User PostedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User UpdatedBy { get; set; }

        #endregion
    }
}