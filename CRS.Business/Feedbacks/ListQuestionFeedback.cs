using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListQuestionFeedback : Feedback
    {
        public ListQuestionFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<Question> Questions { get; set; }
        public int Total { get; set; }
    }
}