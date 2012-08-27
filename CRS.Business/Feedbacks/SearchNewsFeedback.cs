using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class SearchNewsFeedback : Feedback
    {
        public SearchNewsFeedback(bool success, string message = null)
            : base(success, message)
        {
        }

        public IList<News> News { get; set; }
        public int TotalNews { get; set; }
    }
}