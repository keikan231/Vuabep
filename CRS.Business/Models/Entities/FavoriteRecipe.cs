using System;

namespace CRS.Business.Models.Entities
{
    public class FavoriteRecipe
    {
        #region Primitive properties

        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public DateTime AddedDate { get; set; }

        #endregion
    }
}