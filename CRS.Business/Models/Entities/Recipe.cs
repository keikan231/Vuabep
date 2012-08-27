using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class Recipe
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        [StringLengthExtended(100, MinimumLength = 10)]
        [Display(Name = "Recipes_Title", ResourceType = typeof(Labels))]
        public string Title { get; set; }
        public string TitleSearch { get; set; }
        [StringLengthExtended(1000)]
        public string TitleUrl { get; set; }
        [StringLengthExtended(1000)]
        [Display(Name = "Recipes_Image", ResourceType = typeof(Labels))]
        public string ImageUrl { get; set; }
        [RequiredExtended]
        [StringLengthExtended(255, MinimumLength = 10)]
        [Display(Name = "Recipes_ShortDescription", ResourceType = typeof(Labels))]
        public string ContentText { get; set; }
        [RequiredExtended]
        [StringLengthExtended(1000, MinimumLength = 10)]
        [Display(Name = "Recipes_Ingredients", ResourceType = typeof(Labels))]
        public string Ingredients { get; set; }
        [RequiredExtended]
        [StringLengthExtended(int.MaxValue / 2)] // Don't remove this line otherwise EF will throw a validation exception
        [Display(Name = "Recipes_Description", ResourceType = typeof(Labels))]
        [AllowHtml]
        public string ContentHtml { get; set; }

        [RequiredExtended]
        [Column("MappingCategories")]
        [Display(Name = "MappingCategories_Title", ResourceType = typeof(Labels))]
        public int MappingCategoryId { get; set; }
        public int RateTimes { get; set; }
        public int TotalRates { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Reports { get; set; }

        [Column("PostedBy")]
        public int PostedById { get; set; }

        public DateTime PostedDate { get; set; }

        [Column("UpdatedBy")]
        public int? UpdatedById { get; set; }
        public DateTime? LastUpdate { get; set; }

        [Column("ApprovedBy")]
        public int? ApprovedById { get; set; }

        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties
        [ForeignKey("PostedById")]
        public virtual User PostedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User UpdatedBy { get; set; }

        [ForeignKey("ApprovedById")]
        public virtual User ApprovedBy { get; set; }

        [ForeignKey("MappingCategoryId")]
        public virtual RecipeCategoryMapping MappingCategories { get; set; }

        #endregion
    }
}