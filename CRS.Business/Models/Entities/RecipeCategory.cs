using System.Collections.Generic;
using CRS.Common.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class RecipeCategory
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        public string Name { get; set; }
        public string NameUrl { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties

        public virtual ICollection<RecipeCategoryMapping> RecipeCategoryMappings { get; set; }

        #endregion
    }
}