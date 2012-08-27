using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserRecipeViewModel
    {
        public IList<Recipe> Recipes { get; set; }
        public int Total { get; set; }
    }
}