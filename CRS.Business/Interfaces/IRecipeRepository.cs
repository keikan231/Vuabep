using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IRecipeRepository
    {
        Feedback<IList<Recipe>> GetAllReportedRecipes(int minReportNumber);
        Feedback<IList<ReportedRecipe>> GetReportedRecipeDetails(int id);
        Feedback DeleteReportedRecipe(int id);
        Feedback DeleteFalseReports(int id);
        InsertRecipeFeedback InsertRecipe(Recipe q);
        Feedback DeleteRecipe(int id);
        Feedback<Recipe> GetRecipeForEditing(int id);
        RecipeDetailsFeedback GetRecipeDetails(int id);
        Feedback<Recipe> UpdateRecipe(Recipe q);
        Feedback ReportRecipe(ReportedRecipe rt);
        Feedback<Recipe> RateRecipe(RatedRecipe ratedRecipe);
        ListRecipeFeedback GetAllRecipe(SortCriteria criteria);
        SearchRecipeFeedback SearchRecipes(SearchCriteria criteria);
        SearchRecipeFeedback SortRecipes(SortCriteria criteria, int take);
        ListRecipeFeedback GetRecipeByCatogory(int categoryId, SortCriteria criteria, string categoryType, int smallId = 0);
        Feedback AddToFavorite(FavoriteRecipe favoriteRecipe);
        ListRecipeFeedback GetAllUnapprovedRecipe(PageInfo pageInfo);
        Feedback ApproveRecipe(int id);
        Feedback<IList<Recipe>> GetTopRecipes(int take);
        Feedback<IList<Recipe>> GetTopRecipesByCategory(int take, int id);
        ListRecipeFeedback GetAllApprovedRecipe(PageInfo pageInfo);
        Feedback UnapproveRecipe(int id);
    }
}