using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class TagCategory
    {
        #region Primitive properties

        public int Id { get; set; }

        [RequiredExtended]
        [StringLengthExtended(100)]
        [Display(Name = "TagCategory_Name", ResourceType = typeof(Labels))]
        public string Name { get; set; }

        [StringLengthExtended(100)]
        public string NameUrl { get; set; }

        [StringLengthExtended(1000)]
        public string Description { get; set; }
        public int Searches { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties

        #endregion
    }
}