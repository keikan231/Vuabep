using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageCategories
{
    public class ListRecipeCategoryViewModel
    {
        public IList<RecipeCategory> RecipeCategories { get; set; }
    }
}