using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Tips
{
    public class TipDetailsViewModel
    {
        public Tip Tip { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public bool CanEdit { get; set; }
    }
}