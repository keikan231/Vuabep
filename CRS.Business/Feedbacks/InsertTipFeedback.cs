using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class InsertTipFeedback : Feedback
    {
         public InsertTipFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public int NewPoint { get; set; }
        public Tip Tip { get; set; }

    }
}