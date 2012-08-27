namespace CRS.Business.Models.Entities
{
    public class RecipeCategorySelectList
    {
        public int Id { get; set; }
        public int RecipeCategoryId { get; set; }
        public int RecipeSmallCategoryId { get; set; }
        public string RecipeCategoryName { get; set; }
        public string RecipeSmallCategoryName { get; set; }
    }
}