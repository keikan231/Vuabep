using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListNewsFeedback : Feedback
    {
        public ListNewsFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<News> News { get; set; }
        public int Total { get; set; }
    }
}