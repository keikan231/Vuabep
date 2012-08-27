using System.Collections.Generic;
using CRS.Web.Areas.Admin.Models;

namespace CRS.Web.Areas.Admin.Models
{
    public class AdminMenuParent
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public IList<AdminMenuChild> Children { get; set; }

        public AdminMenuParent(string name, string text, IList<AdminMenuChild> children)
        {
            Name = name;
            Text = text;
            Children = children;
        }
    }
}