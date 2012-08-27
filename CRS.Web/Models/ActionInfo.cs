namespace CRS.Web.Models
{
    /// <summary>
    /// Holds info of an action to be called
    /// </summary>
    public class ActionInfo
    {
        public string Text { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int? Id { get; set; }

        public ActionInfo(string text, string action, string controller = null)
        {
            Text = text;
            Action = action;
            Controller = controller;
        }
    }
}