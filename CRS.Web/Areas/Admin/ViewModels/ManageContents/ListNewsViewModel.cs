using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageContents
{
    public class ListNewsViewModel
    {
        public IList<News> News { get; set; }
    }
}