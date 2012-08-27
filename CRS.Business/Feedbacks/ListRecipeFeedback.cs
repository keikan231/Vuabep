using System;
using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class ListRecipeFeedback : Feedback
    {
        public ListRecipeFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<Recipe> Recipes { get; set; }
        public int Total { get; set; }
        public IList<Tuple<int,string>> Categories { get; set; }
    }
}