using System.Web.Mvc;

namespace CRS.Web.Models.Tagging
{
    public interface ITaggable
    {
        string Tag { get; set; }
        int Weight { get; set; }
        string GetLink(UrlHelper urlHelper);
    }
}