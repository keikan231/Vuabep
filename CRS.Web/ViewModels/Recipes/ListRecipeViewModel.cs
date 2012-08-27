using System;
using System.Collections.Generic;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Recipes
{
    public class ListRecipeViewModel
    {
        public IList<Recipe> Recipes { get; set; }
        public Order OrderBy { get; set; }
        public bool HasMore { get; set; }
        public int Page { get; set; }
        public string Sort { get; set; }
        public IList<Tuple<int,string>> Categories { get; set; }
    }
}