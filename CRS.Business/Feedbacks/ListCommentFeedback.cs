using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListCommentFeedback : Feedback
    {
         public ListCommentFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<NewsComment> Comments { get; set; }
        public int Total { get; set; }
    }
}