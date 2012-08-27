using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListTipFeedback : Feedback
    {
        public ListTipFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<Tip> Tips { get; set; }
        public int Total { get; set; }
        public string TipCategoryName { get; set; }
    }
}