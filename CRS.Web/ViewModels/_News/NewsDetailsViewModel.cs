using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels._News
{
    public class NewsDetailsViewModel
    {
        public News News { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public bool CanEdit { get; set; }
    }
}