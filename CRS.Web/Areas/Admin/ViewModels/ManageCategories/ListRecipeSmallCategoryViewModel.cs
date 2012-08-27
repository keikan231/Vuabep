using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageCategories
{
    public class ListRecipeSmallCategoryViewModel
    {
        public IList<RecipeSmallCategory> RecipeSmallCategories { get; set; }
    }
}