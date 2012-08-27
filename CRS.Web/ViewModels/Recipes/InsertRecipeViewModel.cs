using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Recipes
{
    public class InsertRecipeViewModel
    {
        public string OldImageUrl { get; set; }

        public Recipe Recipe { get; set; }

        public HttpPostedFileBase File { get; set; }

        public IDictionary<string, IEnumerable<SelectListItem>> Categories { get; set; }
    }
}