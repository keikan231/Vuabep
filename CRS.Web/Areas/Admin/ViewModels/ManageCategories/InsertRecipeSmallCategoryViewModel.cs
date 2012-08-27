using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageCategories
{
    public class InsertRecipeSmallCategoryViewModel
    {
        public RecipeSmallCategory RecipeSmallCategory { get; set; }

        public IList<CheckedRecipeCategory> RecipeCategory { get; set; }

        public IList<TipCategory> TipCategories { get; set; }
    }
}