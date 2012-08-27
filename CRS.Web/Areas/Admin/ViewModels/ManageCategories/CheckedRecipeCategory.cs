using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageCategories
{
    public class CheckedRecipeCategory
    {
        public bool IsChecked { get; set; }
        public RecipeCategory RecipeCategory { get; set; }
    }
}