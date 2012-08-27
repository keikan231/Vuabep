using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageContents
{
    public class ListRecipesViewModel
    {
        public IList<Recipe> Recipes { get; set; }
    }
}