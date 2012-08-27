using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class RecipeAllCategoryRepository : IRecipeAllCategoryRepository
    {
        public Feedback<IList<RecipeCategorySelectList>> GetAllRecipeCategories()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from m in entities.RecipeCategoryMappings
                                 join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                 join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                 where !c.IsDeleted && !s.IsDeleted
                                 select new
                                            {
                                                MappingId = m.Id,
                                                CategoryId = c.Id,
                                                CategoryName = c.Name,
                                                SmallCategoryId = s.Id,
                                                SmallCategoryName = s.Name
                                            }).ToList();

                    IList<RecipeCategorySelectList> categories = new List<RecipeCategorySelectList>();
                    for (int i = 0; i < query.Count; i++)
                    {
                        categories.Add(new RecipeCategorySelectList
                                           {
                                               Id = query[i].MappingId,
                                               RecipeCategoryId = query[i].CategoryId,
                                               RecipeSmallCategoryId = query[i].SmallCategoryId,
                                               RecipeCategoryName = query[i].CategoryName,
                                               RecipeSmallCategoryName = query[i].SmallCategoryName
                                           });
                    }

                    return new Feedback<IList<RecipeCategorySelectList>>(true, null, categories);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeCategorySelectList>>(false, Messages.GeneralError);
            }
        }
    }
}