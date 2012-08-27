namespace CRS.Business.Models.Entities
{
    public class RecipeCategoryMapping
    {
        #region Primitive properties

        public int Id { get; set; }
        public int RecipeCategoryId { get; set; }
        public int RecipeSmallCategoryId { get; set; }

        #endregion

        #region Navigation properties

        public virtual RecipeCategory RecipeCategory { get; set; }
        public virtual RecipeSmallCategory RecipeSmallCategory { get; set; }

        #endregion
    }
}