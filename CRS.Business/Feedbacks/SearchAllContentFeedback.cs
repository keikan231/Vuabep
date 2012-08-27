using System;
using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class SearchAllContentFeedback : Feedback
    {
        public SearchAllContentFeedback(bool success, string message = null)
            : base(success, message)
        {
        }

        public IList<Tuple<string, News>> All { get; set; }
        public int Total { get; set; }
    }
}