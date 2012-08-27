using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IRecipeCommentRepository
    {
        Feedback<IList<RecipeComment>> GetAllReportedRecipeComments(int minReportNumber);
        Feedback DeleteReportedComment(int id);
        Feedback DeleteFalseReports(int id);
        Feedback<IList<ReportedRecipeComment>> GetReportedCommentDetails(int id);
        Feedback<IList<RecipeComment>> GetRecipeComments(int recipeId, PageInfo pageInfo);
        Feedback<IList<RecipeComment>> GetHighlightRecipeComments(int recipeId, int number);
        Feedback<RecipeComment> InsertRecipeComment(RecipeComment c);
        Feedback DeleteRecipeComment(RecipeComment c);
        Feedback<RecipeComment> VoteRecipeComment(VotedRecipeComment vc);
        Feedback ReportRecipeComment(ReportedRecipeComment rc);
    }
}