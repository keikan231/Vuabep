using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels._News
{
    public class InsertNewsViewModel
    {
        public string OldImageUrl { get; set; }

        public News News { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}