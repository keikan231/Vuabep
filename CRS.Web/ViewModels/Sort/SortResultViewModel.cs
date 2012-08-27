using System.Collections.Generic;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Sort
{
    public class SortResultViewModel
    {
        public bool NewsNotFound { get; set; }
        public bool TipNotFound { get; set; }
        public bool RecipeNotFound { get; set; }
        public IList<News> News { get; set; }
        public IList<Tip> Tips { get; set; }
        public IList<Recipe> Recipes { get; set; }
        public Order OrderBy { get; set; }
        public string Content { get; set; }
    }
}