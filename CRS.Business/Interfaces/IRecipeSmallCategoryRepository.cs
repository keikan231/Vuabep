using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IRecipeSmallCategoryRepository
    {
        Feedback<IList<RecipeSmallCategory>> GetAllRecipeSmallCategories();
        Feedback<RecipeSmallCategory> InsertRecipeSmallCategory(RecipeSmallCategory t, IList<int> recipeCategoryIds);
        Feedback<RecipeSmallCategory> GetRecipeSmallCategoryDetails(int id);
        Feedback<RecipeSmallCategory> UpdateRecipeSmallCategory(RecipeSmallCategory c, IList<int> recipeCategoryIds);
        Feedback DeleteRecipeSmallCategory(int id);
    }
}