using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserFavoriteRecipeViewModel
    {
        public IList<Recipe> FavoriteRecipes { get; set; }
        public int Total { get; set; }
    }
}