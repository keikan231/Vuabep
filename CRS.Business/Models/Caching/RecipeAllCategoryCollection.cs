using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class RecipeAllCategoryCollection : CacheCollection<RecipeCategorySelectList> 
    {
         private IRecipeAllCategoryRepository _repository;

         public RecipeAllCategoryCollection(IRecipeAllCategoryRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<Location>

        public override void Execute()
        {
            Feedback<IList<RecipeCategorySelectList>> feedback = _repository.GetAllRecipeCategories();
            IsPopulated = feedback.Success;
            if (feedback.Success)
            {
                Clear();
                AddRange(feedback.Data);
            }
        }

        #endregion
    }
}