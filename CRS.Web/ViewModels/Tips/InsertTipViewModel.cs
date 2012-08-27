using System.Collections.Generic;
using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Tips
{
    public class InsertTipViewModel
    {
        public string OldImageUrl { get; set; }

        public Tip Tip { get; set; }

        public HttpPostedFileBase File { get; set; }

        public IList<TipCategory> TipCategories { get; set; }

    }
}