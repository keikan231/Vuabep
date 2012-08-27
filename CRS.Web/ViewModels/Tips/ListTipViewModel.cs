using System.Collections.Generic;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Tips
{
    public class ListTipViewModel
    {
        public int TipCategoryId { get; set; }
        public IList<Tip> Tips { get; set; }
        public string TipCategoryName { get; set; }
        public Order OrderBy { get; set; }
        public bool HasMore { get; set; }
        public int Page { get; set; }
        public string Sort { get; set; }
    }
}