using System.Web.Mvc;
using CRS.Web.Helpers;

namespace CRS.Web.Models.Tagging
{
    public class CategoryTag
    {
        #region Implementation of ITaggable

        public string Tag { get; set; }

        public int Weight { get; set; }

        //public string GetLink(UrlHelper urlHelper)
        //{
        //    return urlHelper.CategoryLink(NameUrl);
        //}

        #endregion

        public string NameUrl { get; set; }
    }
}