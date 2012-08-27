using CRS.Common.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class TipCategory
    {
        #region Primitive properties

        public int Id { get; set; }
        [RequiredExtended]
        public string Name { get; set; }
        public string NameUrl { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        #endregion
    }
}