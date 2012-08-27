using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class RecipeSmallCategory
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        public string Name { get; set; }
        public string NameUrl { get; set; }
        public string Description { get; set; }
        public int TipMappingId { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties

        public virtual ICollection<RecipeCategoryMapping> RecipeCategoryMappings { get; set; }

        [ForeignKey("TipMappingId")]
        public virtual TipCategory TipCategories { get; set; }

        #endregion
    }
}