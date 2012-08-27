using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Business.Feedbacks
{
    public class StatisticsFeedback : Feedback
    {
        public StatisticsFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public int AllUserNumber { get; set; }
        public int NewsNumber { get; set; }
        public int TipNumber { get; set; }
        public int RecipeNumber { get; set; }
        public int ApprovedRecipeNumber { get; set; }
        public int TipCategoryNumber { get; set; }
        public int RecipeCategoryNumber { get; set; }
        public int RecipeSmallCategoryNumber { get; set; }
        public int NewsCommentNumber { get; set; }
        public int TipCommentNumber { get; set; }
        public int RecipeCommentNumber { get; set; }
        public int QuestionNumber { get; set; }
        public int AnswerNumber { get; set; }
        public int VisitorNumber { get; set; }
        public int VisitorsToday { get; set; }
    }
}