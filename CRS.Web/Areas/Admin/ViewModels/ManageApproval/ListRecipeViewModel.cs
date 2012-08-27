using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageApproval
{
    public class ListRecipeViewModel
    {
        public IList<Recipe> Recipes { get; set; }
    }
}