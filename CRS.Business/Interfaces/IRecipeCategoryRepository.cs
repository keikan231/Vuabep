using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IRecipeCategoryRepository
    {
        Feedback<IList<RecipeCategory>> GetAllRecipeCategories();
        Feedback<RecipeCategory> InsertRecipeCategory(RecipeCategory t);
        Feedback<RecipeCategory> GetRecipeCategoryDetails(int id);
        Feedback<RecipeCategory> UpdateRecipeCategory(RecipeCategory c);
        Feedback DeleteRecipeCategory(int id);
    }
}