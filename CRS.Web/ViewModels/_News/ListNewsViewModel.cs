using System.Collections.Generic;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels._News
{
    public class ListNewsViewModel
    {
        public IList<News> News { get; set; }
        public bool HasMore { get; set; }
        public int Page { get; set; }
        public Order OrderBy { get; set; }
        public string Sort { get; set; }
    }
}