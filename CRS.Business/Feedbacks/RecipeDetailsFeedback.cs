using System;
using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class RecipeDetailsFeedback : Feedback
    {
        public RecipeDetailsFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public IList<Tuple<int,string>> Categories { get; set; }
        public Recipe Recipe { get; set; }
    }
}