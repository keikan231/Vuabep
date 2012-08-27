using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageCategories
{
    public class ListTipCategoryViewModel
    {
        public IList<TipCategory> TipCategories { get; set; }
    }
}