using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class SearchTipFeedback : Feedback
    {
         public SearchTipFeedback(bool success, string message = null)
            : base(success, message)
        {
        }

        public IList<Tip> Tips { get; set; }
        public int TotalTips { get; set; }
    }
}