using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Web.Models
{
    /// <summary>
    /// Holds info of all main header menu items
    /// </summary>
    public class HeaderMenuCollection
    {
        public static IList<HeaderMenu> Items { get; private set; }

        static HeaderMenuCollection()
        {
            Items = new List<HeaderMenu>();
            CreateParent();
#if DEBUG
            Validate();
#endif
        }


#if DEBUG
        private static void Validate()
        {
            IList<string> parents = new List<string>();
            foreach (var parent in Items)
            {
                // Check to ensure all menu groups have a unique name
                if (parents.Contains(parent.Name))
                    throw new InvalidOperationException(string.Format("2 menus have the same name: {0}", parent.Name));
                parents.Add(parent.Name);

                // Check to ensure all child menus inside a group have a unique name
                IList<string> children = new List<string>();
                foreach (var child in parent.Children)
                {
                    if (children.Contains(child.Name))
                        throw new InvalidOperationException(string.Format("2 menus have the same name: {0}", child.Name));
                    children.Add(child.Name);
                }
            }
        }
#endif

        private static void CreateParent()
        {
            Items.Add(new HeaderMenu("Home", "Trang chủ", "Index", "Home"));
            Items.Add(new HeaderMenu("News", "Tin tức", "Index", "News"));
            Items.Add(new HeaderMenu("Recipes", "Công thức", "Index", "Recipes"));
            Items.Add(new HeaderMenu("Tips", "Mẹo vặt", "Index", "Tips"));
            Items.Add(new HeaderMenu("Q&A", "Hỏi Đáp", "Index", "Questions"));
            Items.Add(new HeaderMenu("Search", "Tìm kiếm", "Index", "Search"));
        }
    }
}