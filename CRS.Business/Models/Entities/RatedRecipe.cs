using System;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class RatedRecipe
    {
        #region Primitive properties

        public int Id { get; set; }
        public int RecipeId { get; set; }
        [RangeExtended(1, 5)]
        public int Rate { get; set; }
        [Column("RatedBy")]
        public int RatedById { get; set; }
        public DateTime RatedDate { get; set; }

        #endregion

        #region Navigation properties

        public Recipe Recipe { get; set; }
        [ForeignKey("RatedById")]
        public User RatedBy { get; set; }

        #endregion
    }
}