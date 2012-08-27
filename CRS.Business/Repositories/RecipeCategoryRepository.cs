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
    public class RecipeCategoryRepository : IRecipeCategoryRepository
    {
        public Feedback<IList<RecipeCategory>> GetAllRecipeCategories()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var categories =
                        entities.RecipeCategories.Where(t => !t.IsDeleted).OrderBy(t => t.Id).ToList();
                    return new Feedback<IList<RecipeCategory>>(true, null, categories);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeCategory>>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeCategory> InsertRecipeCategory(RecipeCategory t)
        {
            RecipeCategory tnew = new RecipeCategory
            {
                Name = t.Name,
                NameUrl = t.NameUrl,
                Description = t.Description,
                IsDeleted = false
            };

            try
            {
                using (var entities = new CrsEntities())
                {
                    //Check for duplicate name
                    var exist = entities.RecipeCategories
                        .FirstOrDefault(i => i.Name == tnew.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<RecipeCategory>(false, Messages.InsertCategory_DuplicateName);

                    entities.RecipeCategories.Add(tnew);
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
                        exist = entities.RecipeCategories.FirstOrDefault(
                                i => i.Id != tnew.Id && i.NameUrl == tnew.NameUrl && !i.IsDeleted);
                        if (exist != null)
                        {
                            tnew.NameUrl = string.Format("{0}-{1}", tnew.NameUrl, tnew.Id);
                            entities.SaveChanges();
                        }
                    }
                }
                return new Feedback<RecipeCategory>(true, Messages.InsertRecipeCategorySuccess, tnew);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeCategory> GetRecipeCategoryDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    RecipeCategory category = entities.RecipeCategories.SingleOrDefault(i => i.Id == id && !i.IsDeleted);
                    if (category != null)
                        return new Feedback<RecipeCategory>(true, null, category);
                    else
                        return new Feedback<RecipeCategory>(false, Messages.GetCategory_NotFound);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeCategory> UpdateRecipeCategory(RecipeCategory c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    RecipeCategory exist = entities.RecipeCategories.FirstOrDefault(i => i.Id != c.Id && i.Name == c.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<RecipeCategory>(false, Messages.InsertCategory_DuplicateName);

                    var category = entities.RecipeCategories.Single(i => i.Id == c.Id && !i.IsDeleted);
                    category.Name = c.Name;
                    category.Description = c.Description;

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(c.NameUrl))
                    {
                        category.NameUrl = c.Id.ToString();
                    }
                    else
                    {
                        exist = entities.RecipeCategories.FirstOrDefault(
                            i => i.Id != c.Id && i.NameUrl == c.NameUrl && !i.IsDeleted);
                        category.NameUrl = exist != null
                                               ? string.Format("{0}-{1}", c.NameUrl, c.Id)
                                               : c.NameUrl;
                    }

                    entities.SaveChanges();

                    return new Feedback<RecipeCategory>(true, null, category);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteRecipeCategory(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    RecipeCategory c = entities.RecipeCategories.Single(i => i.Id == id);
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