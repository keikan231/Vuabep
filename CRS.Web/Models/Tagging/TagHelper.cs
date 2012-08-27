using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CRS.Web.Models.Tagging
{
    public static class TagHelper
    {
        private const string TagHtmlTemplate = "<a class=\"tag-{0}\" href=\"{1}\">{2}</a>\r\n";

        public static MvcHtmlString TagCloud(this HtmlHelper html, IEnumerable<ITaggable> items, int numberOfStyleVariations)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Count() == 0)
                return new MvcHtmlString(string.Empty);

            var sorted = items.OrderBy(x => x.Tag, StringComparer.InvariantCultureIgnoreCase);
            int min = sorted.Min(x => x.Weight) * numberOfStyleVariations;
            int max = sorted.Max(x => x.Weight) * numberOfStyleVariations;
            int distribution = (max - min) / numberOfStyleVariations;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div class=\"tag-cloud\">");

            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            foreach (var taggable in sorted)
            {
                int w = taggable.Weight * numberOfStyleVariations;
                for (double i = min, j = 1; i <= max; i += distribution, j++)
                {
                    if (w >= i && w <= i + distribution)
                    {
                        string link = taggable.GetLink(urlHelper);
                        sb.AppendFormat(TagHtmlTemplate, j, link, taggable.Tag);
                        break;
                    }
                }
            }
            sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}