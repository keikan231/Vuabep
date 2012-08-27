using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class RecipeCategoryCollection : CacheCollection<RecipeCategory>
    {
          private readonly IRecipeCategoryRepository _repository;

          public RecipeCategoryCollection(IRecipeCategoryRepository repository)
            {
                _repository = repository;
            }

            public override void Execute()
            {
                Feedback<IList<RecipeCategory>> feedback = _repository.GetAllRecipeCategories();
                IsPopulated = feedback.Success;
                if (feedback.Success)
                {
                    Clear();
                    AddRange(feedback.Data);
                }
        }
    }
}