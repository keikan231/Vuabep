using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class InsertNewsFeedback: Feedback
    {
        public InsertNewsFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public int NewPoint { get; set; }
        public News News { get; set; }
    }
}