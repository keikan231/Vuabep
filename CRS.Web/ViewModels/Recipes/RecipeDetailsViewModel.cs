using System;
using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Recipes
{
    public class RecipeDetailsViewModel
    {
        public Recipe Recipe { get; set; }
        public IList<Tuple<int,string>> Categories { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public bool CanEdit { get; set; }
    }
}