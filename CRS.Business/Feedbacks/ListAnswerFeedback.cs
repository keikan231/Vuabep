using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListAnswerFeedback : Feedback
    {
        public ListAnswerFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<Answer> Answers { get; set; }
        public int Total { get; set; }
    }
}