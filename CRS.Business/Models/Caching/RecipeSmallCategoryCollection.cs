using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class RecipeSmallCategoryCollection : CacheCollection<RecipeSmallCategory>
    {
        private readonly IRecipeSmallCategoryRepository _repository;

        public RecipeSmallCategoryCollection(IRecipeSmallCategoryRepository repository)
            {
                _repository = repository;
            }

            public override void Execute()
            {
                Feedback<IList<RecipeSmallCategory>> feedback = _repository.GetAllRecipeSmallCategories();
                IsPopulated = feedback.Success;
                if (feedback.Success)
                {
                    Clear();
                    AddRange(feedback.Data);
                }
        }
    }
}