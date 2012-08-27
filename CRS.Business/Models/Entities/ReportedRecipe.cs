using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class ReportedRecipe
    {
        #region Primitive properties

        public int Id { get; set; }
        public int RecipeId { get; set; }
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
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }

        #endregion
    }
}