using System;
using System.Collections.Generic;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Web.Models.Searching;

namespace CRS.Web.ViewModels.Search
{
    public class SearchResultViewModel
    {
        public bool NotFound { get; set; }
        public SearchInput Input { get; set; }
        public IList<News> News { get; set; }
        public IList<Tuple<string, News>> All { get; set; }
        public IList<Recipe> Recipes { get; set; }
        public IList<Tip> Tips { get; set; }
        public int Total { get; set; }
        public bool HasMore { get; set; }
        public Order OrderBy { get; set; }
    }
}