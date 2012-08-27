using System.Collections.Generic;

namespace CRS.Web.Models
{
    public class HeaderMenu
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public IList<HeaderMenu> Children { get; set; }

        public HeaderMenu(string name, string text, string action, string controller)
        {
            Name = name;
            Text = text;
            Controller = controller;
            Action = action;
            Children = new List<HeaderMenu>();
        }
    }
}