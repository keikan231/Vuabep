using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace CRS.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Splits this string using the specified separator. Trims all substrings and removes null or empty values.
        /// </summary>
        public static string[] SplitAndTrim(this string s, char separator)
        {
            if (s == null)
                return new string[0];

            return s.Split(separator)
                .Where(i => i != null && i.Trim() != string.Empty)
                .Select(i => i.Trim())
                .ToArray();
        }

        /// <summary>
        /// Truncates this string to fit in the specified length
        /// </summary>
        public static string Truncate(this string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            if (maxLength > 3 && s.Length > maxLength)
            {
                return s.Substring(0, maxLength - 3) + "...";
            }

            return s;
        }

        /// <summary>
        /// Truncates a word which is more than the specified length
        /// </summary>
        public static string TruncateWord(this string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            string[] words = s.Split(' ');
            foreach (string word in words)
            {
                if(maxLength > 3 && word.Length > maxLength)
                {
                    return s.Substring(0, maxLength - 3) + "...";
                }
            }

            return s;
        }

        /// <summary>
        /// Converts this string to be URL-friendly
        /// </summary>
        public static string ToUrlFriendly(this string s)
        {
            if (s == null) return s;

            s = s.Normalize(NormalizationForm.FormD);
            s = s.Trim().ToLower().Replace('đ', 'd');

            var result = new StringBuilder();
            bool flag = false;
            foreach (var t in s.Where(t => t <= 300))
            {
                if (t >= 'a' && t <= 'z' || t >= '0' && t <= '9')
                {
                    result.Append(t);
                    flag = true;
                }
                else if (flag)
                {
                    result.Append('-');
                    flag = false;
                }

            }

            // Remove last - character if neccessary
            if (result.Length > 0 && result[result.Length - 1] == '-')
                result.Remove(result.Length - 1, 1);


            return result.ToString();
        }

        /// <summary>
        /// Converts this string to be search-friendly:
        ///     - Convert to contain non-unicode characters only
        ///     - Convert all characters to lower case
        ///     - Remove all non-alphanumeric characters
        /// </summary>
        public static string ToSearchFriendly(this string s)
        {
            if (s == null) return s;

            s = s.Normalize(NormalizationForm.FormD);
            s = s.Trim().ToLower().Replace('đ', 'd');

            var result = new StringBuilder();
            foreach (var t in s.Where(t => t <= 300))
            {
                if (t >= 'a' && t <= 'z' || t >= '0' && t <= '9')
                    result.Append(t);
            }

            return result.ToString();
        }

        /// <summary>
        /// Removes all Html tags in this string
        /// </summary>
        public static string RemoveHtml(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            s = Regex.Replace(s, "<[^>]*>", string.Empty);
            s = Regex.Replace(s, @"[\s\r\n]+", " ");

            return s.Trim();
        }

         //<summary>
         //Removes html tags or attributes that are not specified in the white list
         //</summary>
        public static string GetSafeHtml(this string html, string[] tagWhitelist, string[] attributeWhiteList)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            HtmlDocument doc = new HtmlDocument();
            // Treat br tag in HTML 4 way
            HtmlNode.ElementsFlags["br"] = HtmlElementFlag.Empty;
            doc.OptionWriteEmptyNodes = true;
            doc.LoadHtml(html);

            IList<HtmlNode> hnc = doc.DocumentNode.DescendantNodes().ToList();

            // Remove non-white list nodes
            for (int i = hnc.Count - 1; i >= 0; i--)
            {
                HtmlNode htmlNode = hnc[i];
                if (!tagWhitelist.Contains(htmlNode.Name.ToLower()))
                {
                    htmlNode.Remove();
                    continue;
                }

                // Dont need to encode unicode text
                if (htmlNode.Name.ToLower() == "#text")
                {
                    StringBuilder sb = new StringBuilder(HttpUtility.HtmlDecode(htmlNode.InnerHtml));
                    sb.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                    htmlNode.InnerHtml = sb.ToString();
                }

                for (int att = htmlNode.Attributes.Count - 1; att >= 0; att--)
                {
                    HtmlAttribute attribute = htmlNode.Attributes[att];
                    // Remove any attribute that is not in the white list (such as event handlers)
                    if (!attributeWhiteList.Contains(attribute.Name.ToLower()))
                    {
                        attribute.Remove();
                    }

                    // Strip any "style" attributes that contain the word "expression"
                    if (attribute.Value.ToLower().Contains("expression") && attribute.Name.ToLower() == "style")
                    {
                        attribute.Value = string.Empty;
                    }

                    if (attribute.Name.ToLower() == "src" || attribute.Name.ToLower() == "href")
                    {
                        string val = attribute.Value.ToLower();
                        // Strip if the link refs to script
                        if (val.StartsWith("javascript") || val.StartsWith("jscript") || val.StartsWith("vbscript"))
                            attribute.Value = "#";
                    }
                }
            }

            return doc.DocumentNode.WriteTo();
        }

        public static string ConvertUrlToLink(this string s)
        {
            if (s == null)
                return s;

            Regex r = new Regex("(https?://[^ ]+)");
            s = r.Replace(s, "<a href=\"$1\">$1</a>");

            return s;
        }
    }
}