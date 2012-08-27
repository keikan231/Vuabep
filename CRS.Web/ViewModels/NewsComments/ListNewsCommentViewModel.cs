using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.NewsComments
{
    public class ListNewsCommentViewModel
    {
        public int NewsId { get; set; }
        public IList<NewsComment> Comments { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public IList<NewsComment> HighlightComments { get; set; }
    }
}