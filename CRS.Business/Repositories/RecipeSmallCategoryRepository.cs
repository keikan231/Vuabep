using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class RecipeSmallCategoryRepository : IRecipeSmallCategoryRepository
    {
        public Feedback<IList<RecipeSmallCategory>> GetAllRecipeSmallCategories()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var categories =
                        entities.RecipeSmallCategories.Include("RecipeCategoryMappings.RecipeCategory").Where(t => !t.IsDeleted).OrderBy(t => t.Id).ToList();
                    return new Feedback<IList<RecipeSmallCategory>>(true, null, categories);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeSmallCategory>>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeSmallCategory> InsertRecipeSmallCategory(RecipeSmallCategory t, IList<int> recipeCategoryIds)
        {
            RecipeSmallCategory tnew = new RecipeSmallCategory
            {
                Name = t.Name,
                NameUrl = t.NameUrl,
                Description = t.Description,
                TipMappingId = t.TipMappingId,
                IsDeleted = false
            };

            try
            {
                using (var entities = new CrsEntities())
                {
                    //Check for duplicate name
                    var exist = entities.RecipeSmallCategories
                        .FirstOrDefault(i => i.Name == tnew.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<RecipeSmallCategory>(false, Messages.InsertCategory_DuplicateName);

                    entities.RecipeSmallCategories.Add(tnew);

                    if (recipeCategoryIds == null)
                        recipeCategoryIds = new List<int>();
                    foreach (int itemId in recipeCategoryIds)
                    {
                        RecipeCategoryMapping rp = new RecipeCategoryMapping
                        {
                            RecipeCategoryId = itemId,
                            RecipeSmallCategoryId = t.Id
                        };
                        entities.RecipeCategoryMappings.Add(rp);
                    }

                    entities.SaveChanges();

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(t.NameUrl))
                    {
                        tnew.NameUrl = tnew.Id.ToString();
                        entities.SaveChanges();
                    }
                    else
                    {
                        exist = entities.RecipeSmallCategories.FirstOrDefault(
                                i => i.Id != tnew.Id && i.NameUrl == tnew.NameUrl && !i.IsDeleted);
                        if (exist != null)
                        {
                            tnew.NameUrl = string.Format("{0}-{1}", tnew.NameUrl, tnew.Id);
                            entities.SaveChanges();
                        }
                    }


                }
                return new Feedback<RecipeSmallCategory>(true, Messages.InsertRecipeSmallCategorySuccess, tnew);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeSmallCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeSmallCategory> GetRecipeSmallCategoryDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    RecipeSmallCategory category = entities.RecipeSmallCategories.Include("RecipeCategoryMappings").SingleOrDefault(i => i.Id == id && !i.IsDeleted);
                    if (category != null)
                        return new Feedback<RecipeSmallCategory>(true, null, category);
                    else
                        return new Feedback<RecipeSmallCategory>(false, Messages.GetCategory_NotFound);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeSmallCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeSmallCategory> UpdateRecipeSmallCategory(RecipeSmallCategory c, IList<int> recipeCategoryIds)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    RecipeSmallCategory exist = entities.RecipeSmallCategories.FirstOrDefault(i => i.Id != c.Id && i.Name == c.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<RecipeSmallCategory>(false, Messages.InsertCategory_DuplicateName);

                    var category = entities.RecipeSmallCategories.Single(i => i.Id == c.Id && !i.IsDeleted);
                    category.Name = c.Name;
                    category.Description = c.Description;
                    category.TipMappingId = c.TipMappingId;

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(c.NameUrl))
                    {
                        category.NameUrl = c.Id.ToString();
                    }
                    else
                    {
                        exist = entities.RecipeSmallCategories.FirstOrDefault(
                            i => i.Id != c.Id && i.NameUrl == c.NameUrl && !i.IsDeleted);
                        category.NameUrl = exist != null
                                               ? string.Format("{0}-{1}", c.NameUrl, c.Id)
                                               : c.NameUrl;
                    }

                    //Remove from RecipeCategoryMapping
                    foreach (var a in entities.RecipeCategoryMappings.Where(t => t.RecipeSmallCategoryId == c.Id).ToList())
                        entities.RecipeCategoryMappings.Remove(a);

                    //Add to DB
                    var recipeSmallCategory = entities.RecipeSmallCategories.Single(i => i.Id == c.Id);
                    recipeSmallCategory.Name = c.Name;
                    recipeSmallCategory.Description = c.Description;

                    foreach (int itemId in recipeCategoryIds)
                    {
                        RecipeCategoryMapping rp = new RecipeCategoryMapping
                        {
                            RecipeCategoryId = itemId,
                            RecipeSmallCategoryId = recipeSmallCategory.Id
                        };
                        entities.RecipeCategoryMappings.Add(rp);
                    }

                    entities.SaveChanges();

                    return new Feedback<RecipeSmallCategory>(true, null, category);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeSmallCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteRecipeSmallCategory(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    RecipeSmallCategory c = entities.RecipeSmallCategories.Single(i => i.Id == id);
                    c.IsDeleted = true;
                    entities.SaveChanges();

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.GeneralError);
            }
        }
    }
}