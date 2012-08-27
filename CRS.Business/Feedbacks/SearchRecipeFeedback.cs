using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class SearchRecipeFeedback : Feedback
    {
        public SearchRecipeFeedback(bool success, string message = null)
            : base(success, message)
        {
        }

        public IList<Recipe> Recipes { get; set; }
        public int TotalRecipes { get; set; }
    }
}