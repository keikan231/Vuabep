using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IRecipeAllCategoryRepository
    {
        Feedback<IList<RecipeCategorySelectList>> GetAllRecipeCategories();
    }
}