namespace CRS.Web.Areas.Admin.Models
{
    public class AdminMenuChild
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RequiredPermission { get; set; }

        public AdminMenuChild(string name, string text, string action, string controller, string requiredPermission)
        {
            Name = name;
            Text = text;
            Controller = controller;
            Action = action;
            RequiredPermission = requiredPermission;
        }
    }
}