using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.LevelManagement;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common.Helpers;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        #region Reported Recipes

        public Feedback<IList<Recipe>> GetAllReportedRecipes(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from q in entities.Recipes
                                    from u in entities.Users
                                    where !q.IsDeleted && q.Reports >= minReportNumber && q.PostedById == u.Id
                                    orderby q.Reports descending
                                    select new
                                    {
                                        q.Id,
                                        q.Title,
                                        q.TitleUrl,
                                        q.Reports,
                                        q.PostedDate,
                                        q.PostedById,
                                        u.Username
                                    };
                    List<Recipe> rq = new List<Recipe>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new Recipe
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            PostedDate = q.PostedDate,
                            Reports = q.Reports,
                            PostedBy = new User
                            {
                                Id = q.PostedById,
                                Username = q.Username
                            },
                        });
                    }

                    return new Feedback<IList<Recipe>>(true, null, rq);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Recipe>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<ReportedRecipe>> GetReportedRecipeDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedRecipes.Where(i => i.RecipeId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedRecipe> rq = new List<ReportedRecipe>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new ReportedRecipe
                        {
                            Id = q.Id,
                            ReportedDate = q.ReportedDate,
                            Reason = q.Reason,
                            ReportedBy = new User
                            {
                                Username = q.Username
                            }
                        });
                    }
                    return new Feedback<IList<ReportedRecipe>>(true, null, rq);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedRecipe>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedRecipe(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get Recipe needed to be deleted
                    Recipe q = entities.Recipes.Single(i => i.Id == id);
                    q.IsDeleted = true;
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

        public Feedback DeleteFalseReports(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Delete attached reports of the quesion
                    List<ReportedRecipe> rq = entities.ReportedRecipes.Where(i => i.RecipeId == id).ToList();
                    foreach (ReportedRecipe i in rq)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    Recipe q = entities.Recipes.Single(i => i.Id == id);
                    q.Reports = 0;
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

        #endregion

        #region Recipes

        public InsertRecipeFeedback InsertRecipe(Recipe q)
        {
            try
            {
                var recipe = new Recipe
                {
                    Title = q.Title,
                    TitleSearch = q.Title.ToSearchFriendly(),
                    TitleUrl = q.TitleUrl,
                    ImageUrl = q.ImageUrl,
                    Ingredients = q.Ingredients,
                    ContentHtml = q.ContentHtml,
                    ContentText = q.ContentText,
                    MappingCategoryId = q.MappingCategoryId,
                    Views = 0,
                    Comments = 0,
                    PostedById = q.PostedById,
                    PostedDate = DateTime.Now,
                    IsDeleted = false,
                    UpdatedById = null,
                    LastUpdate = null,
                    IsApproved = false,
                    ApprovedById = null
                };
                using (var entities = new CrsEntities())
                {
                    // Add to DB
                    entities.Recipes.Add(recipe);
                    entities.SaveChanges();

                    var user = entities.Users.Single(t => t.Id == recipe.PostedById);
                    if (user.TodayDate != DateTime.Today)
                    {
                        user.TodayDate = DateTime.Today;
                        user.TodayPoint = 0;
                    }

                    var createRecipePoint = ReferenceDataCache.PointConfigCollection.CreateRecipe;
                    int maxPointPerDay = ReferenceDataCache.PointConfigCollection.MaxPointPerDay;

                    if (user.TodayPoint < maxPointPerDay)
                    {
                        if (user.TodayPoint + createRecipePoint > maxPointPerDay)
                        {
                            user.Point += maxPointPerDay - user.TodayPoint.Value;
                            user.TodayPoint = maxPointPerDay;
                        }
                        else
                        {
                            user.Point += createRecipePoint;
                            user.TodayPoint += createRecipePoint;
                        }
                    }

                    user.Level = LevelHandler.CurrentLevel(user.Point);

                    //Update data
                    entities.SaveChanges();

                    return new InsertRecipeFeedback(true, Messages.InsertRecipeSuccess) { Recipe = recipe, NewPoint = user.Point };
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new InsertRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public ListRecipeFeedback GetAllRecipe(SortCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Recipes
                                join u in entities.Users on q.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                where !q.IsDeleted && !s.IsDeleted && !c.IsDeleted
                                orderby q.PostedDate descending
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.ImageUrl,
                                    q.MappingCategoryId,
                                    q.PostedById,
                                    q.PostedDate,
                                    q.RateTimes,
                                    q.TotalRates,
                                    q.Views,
                                    q.Comments,
                                    q.ContentHtml,
                                    q.ContentText,
                                    q.IsApproved,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();

                    //Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                            break;
                        case Order.Rates:
                            query = query.OrderByDescending(i => (i.RateTimes == 0? 0 :(i.TotalRates / i.RateTimes))).ThenByDescending(i => i.Views);
                            break;
                        case Order.Recent:
                            query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                            break;
                        case Order.Approval:
                            query = query.Where(i => i.IsApproved).OrderByDescending(i => i.Views);
                            break;
                    }

                    query = query.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);
                    var recipes = new List<Recipe>();
                    foreach (var q in query)
                    {
                        recipes.Add(new Recipe()
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            MappingCategories = new RecipeCategoryMapping { Id = q.MappingCategoryId },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            RateTimes = q.RateTimes,
                            TotalRates = q.TotalRates,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentHtml = q.ContentHtml,
                            ContentText = q.ContentText,
                            IsApproved = q.IsApproved
                        });
                    }

                    return new ListRecipeFeedback(true, null) { Recipes = recipes, Total = total };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public ListRecipeFeedback GetRecipeByCatogory(int categoryId, SortCriteria criteria, string categoryType, int smallId = 0)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    int total = 0;

                    IList<Recipe> recipes = new List<Recipe>();
                    IList<Tuple<int, string>> categories = new List<Tuple<int, string>>();

                    string categoryName = "Công thức nấu ăn";
                    try
                    {
                        categoryName =
                        entities.RecipeCategories.FirstOrDefault(i => i.Id == categoryId && !i.IsDeleted).Name;
                    }
                    catch (Exception e)
                    {
                        return new ListRecipeFeedback(false, Messages.CategoryNotExisted);
                    }

                    categories.Add(new Tuple<int, string>
                                           (
                                               categoryId,
                                               categoryName
                                           ));

                    if (categoryType == "big")
                    {
                        var query2 = from q in entities.Recipes
                                     join u in entities.Users on q.PostedById equals u.Id
                                     join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                     join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                     join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                     where !q.IsDeleted && !c.IsDeleted && !s.IsDeleted && c.Id == categoryId
                                     select new
                                                {
                                                    q.Id,
                                                    q.Title,
                                                    q.TitleUrl,
                                                    q.ImageUrl,
                                                    q.PostedById,
                                                    q.PostedDate,
                                                    q.RateTimes,
                                                    q.TotalRates,
                                                    q.Views,
                                                    q.Comments,
                                                    q.ContentHtml,
                                                    q.ContentText,
                                                    q.IsApproved,
                                                    UserId = u.Id,
                                                    u.Username,
                                                };

                        total = query2.Count();

                        //Sorting
                        switch (criteria.OrderBy)
                        {
                            case Order.Views:
                                query2 = query2.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                                break;
                            case Order.Comments:
                                query2 = query2.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                                break;
                            case Order.Rates:
                                query2 =
                                    query2.OrderByDescending(i => i.RateTimes == 0? 0 :(i.TotalRates / i.RateTimes)).ThenByDescending(
                                        i => i.Views);
                                break;
                            case Order.Recent:
                                query2 = query2.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                                break;
                            case Order.Approval:
                                query2 = query2.Where(i => i.IsApproved).OrderByDescending(i => i.Views);
                                break;
                        }

                        query2 = query2.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);

                        foreach (var q in query2)
                        {
                            recipes.Add(new Recipe
                            {
                                Id = q.Id,
                                Title = q.Title,
                                TitleUrl = q.TitleUrl,
                                ImageUrl = q.ImageUrl,
                                PostedBy = new User { Id = q.UserId, Username = q.Username },
                                PostedDate = q.PostedDate,
                                RateTimes = q.RateTimes,
                                TotalRates = q.TotalRates,
                                Views = q.Views,
                                Comments = q.Comments,
                                ContentHtml = q.ContentHtml,
                                ContentText = q.ContentText,
                                IsApproved = q.IsApproved
                            });
                        }
                    }
                    else if (categoryType == "small")
                    {
                        var query3 = from q in entities.Recipes
                                     join u in entities.Users on q.PostedById equals u.Id
                                     join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                     join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                     join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                     where !q.IsDeleted && !c.IsDeleted && !s.IsDeleted && s.Id == smallId && m.RecipeCategoryId == categoryId
                                     select new
                                     {
                                         q.Id,
                                         q.Title,
                                         q.TitleUrl,
                                         q.ImageUrl,
                                         q.MappingCategoryId,
                                         q.PostedById,
                                         q.PostedDate,
                                         q.RateTimes,
                                         q.TotalRates,
                                         q.Views,
                                         q.Comments,
                                         q.ContentHtml,
                                         q.ContentText,
                                         q.IsApproved,
                                         UserId = u.Id,
                                         u.Username,
                                         s.Name
                                     };

                        total = query3.Count();

                        //Sorting
                        switch (criteria.OrderBy)
                        {
                            case Order.Views:
                                query3 = query3.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                                break;
                            case Order.Comments:
                                query3 = query3.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                                break;
                            case Order.Rates:
                                query3 =
                                    query3.OrderByDescending(i => i.RateTimes == 0 ? 0 : (i.TotalRates / i.RateTimes)).ThenByDescending(
                                        i => i.Views);
                                break;
                            case Order.Recent:
                                query3 = query3.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                                break;
                            case Order.Approval:
                                query3 = query3.Where(i => i.IsApproved).OrderByDescending(i => i.Views);
                                break;
                        }

                        query3 = query3.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);

                        foreach (var q in query3)
                        {
                            recipes.Add(new Recipe
                            {
                                Id = q.Id,
                                Title = q.Title,
                                TitleUrl = q.TitleUrl,
                                ImageUrl = q.ImageUrl,
                                PostedBy = new User { Id = q.UserId, Username = q.Username },
                                PostedDate = q.PostedDate,
                                RateTimes = q.RateTimes,
                                TotalRates = q.TotalRates,
                                Views = q.Views,
                                Comments = q.Comments,
                                ContentHtml = q.ContentHtml,
                                ContentText = q.ContentText,
                                IsApproved = q.IsApproved
                            });
                        }

                        string smallCategoryName = "Công thức nấu ăn";
                        try
                        {
                            smallCategoryName =
                                entities.RecipeSmallCategories.FirstOrDefault(i => i.Id == smallId && !i.IsDeleted).Name;
                        }
                        catch (Exception e)
                        {
                            return new ListRecipeFeedback(false, Messages.CategoryNotExisted);
                        }

                        categories.Add(new Tuple<int, string>
                                           (
                                               smallId,
                                               smallCategoryName
                                           ));
                    }

                    var query4 = (from m in entities.RecipeCategoryMappings
                                  join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                  join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                  where !c.IsDeleted && !s.IsDeleted && c.Id == categoryId
                                  orderby c.Id
                                  select new
                                  {
                                      CategorySmallId = s.Id,
                                      CategorySmallName = s.Name
                                  }
                                   ).ToList();

                    if (query4.Count > 0)
                    {
                        foreach (var item in query4)
                        {
                            categories.Add(new Tuple<int, string>
                                       (
                                           item.CategorySmallId,
                                           item.CategorySmallName
                                       ));
                        }
                    }

                    return new ListRecipeFeedback(true, null) { Recipes = recipes, Total = total, Categories = categories };
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteRecipe(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Recipe q = entities.Recipes.Single(i => i.Id == id);
                    q.IsDeleted = true;
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

        public Feedback<Recipe> GetRecipeForEditing(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var recipe = entities.Recipes.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    return recipe != null ? new Feedback<Recipe>(true, null, recipe) : new Feedback<Recipe>(false, Messages.RecipeNotExisted);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Recipe>(false, Messages.GeneralError);
            }
        }

        public RecipeDetailsFeedback GetRecipeDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from q in entities.Recipes
                                 join u1 in entities.Users on q.PostedById equals u1.Id
                                 join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                 join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                 join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                 where !q.IsDeleted && !s.IsDeleted && !c.IsDeleted && q.Id == id
                                 select new
                                 {
                                     RecipeId = q.Id,
                                     q.Title,
                                     q.TitleUrl,
                                     RecipeImageUrl = q.ImageUrl,
                                     RecipePostedDate = q.PostedDate,
                                     RecipePostedById = q.PostedById,
                                     RecipeContentHtml = q.ContentHtml,
                                     RecipeContentText = q.ContentText,
                                     RecipeCom = q.Comments,
                                     RecipeViews = q.Views,
                                     RecipeCat = q.MappingCategoryId,
                                     RecipeIngredient = q.Ingredients,
                                     RecipeUsername = u1.Username,
                                     RecipeAvatarUrl = u1.AvatarUrl,
                                     RecipeLevel = u1.Level,
                                     RecipeRateTimes = q.RateTimes,
                                     RecipeTotalRates = q.TotalRates,
                                     RecipeApprove = q.IsApproved
                                 }).ToList();
                    if (query.Count <= 0) return new RecipeDetailsFeedback(false, Messages.RecipeNotExisted);

                    var recipe = new Recipe
                    {
                        Id = query[0].RecipeId,
                        Title = query[0].Title,
                        TitleUrl = query[0].TitleUrl,
                        ImageUrl = query[0].RecipeImageUrl,
                        PostedById = query[0].RecipePostedById,
                        PostedDate = query[0].RecipePostedDate,
                        PostedBy = new User { Id = query[0].RecipePostedById, Username = query[0].RecipeUsername, AvatarUrl = query[0].RecipeAvatarUrl, Level = query[0].RecipeLevel },
                        ContentHtml = query[0].RecipeContentHtml,
                        ContentText = query[0].RecipeContentText,
                        MappingCategoryId = query[0].RecipeCat,
                        Ingredients = query[0].RecipeIngredient,
                        Comments = query[0].RecipeCom,
                        Views = query[0].RecipeViews,
                        RateTimes = query[0].RecipeRateTimes,
                        TotalRates = query[0].RecipeTotalRates,
                        IsApproved = query[0].RecipeApprove
                    };

                    var tmpQues = entities.Recipes.Single(i => i.Id == id);
                    tmpQues.Views++;

                    // Show all recipe categories

                    var mappingId = query[0].RecipeCat;

                    var query2 = (from m in entities.RecipeCategoryMappings
                                  join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                  join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                  where !c.IsDeleted && !s.IsDeleted && m.Id == mappingId
                                  select new
                                  {
                                      CategoryId = c.Id,
                                      CategoryName = c.Name,
                                      CategorySmallId = s.Id,
                                      CategorySmallName = s.Name,
                                  }).ToList();

                    IList<Tuple<int, string>> categories = new List<Tuple<int, string>>();

                    if (query2.Count > 0)
                    {
                        categories.Add(new Tuple<int, string>
                                           (
                                               query2[0].CategoryId,
                                               query2[0].CategoryName
                                           ));

                        categories.Add(new Tuple<int, string>
                                           (
                                               query2[0].CategorySmallId,
                                               query2[0].CategorySmallName
                                           ));

                        int categoryId = query2[0].CategoryId;

                        var query3 = (from m in entities.RecipeCategoryMappings
                                      join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                      join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                      where !c.IsDeleted && !s.IsDeleted && c.Id == categoryId
                                      orderby c.Id
                                      select new
                                      {
                                          CategorySmallId = s.Id,
                                          CategorySmallName = s.Name
                                      }
                                     ).ToList();

                        if (query3.Count > 0)
                        {
                            foreach (var item in query3)
                            {
                                categories.Add(new Tuple<int, string>
                                          (
                                              item.CategorySmallId,
                                              item.CategorySmallName
                                          ));
                            }
                        }
                    }

                    entities.SaveChanges();

                    return new RecipeDetailsFeedback(true, null) { Categories = categories, Recipe = recipe };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new RecipeDetailsFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback<Recipe> RateRecipe(RatedRecipe ratedRecipe)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var exist = entities.RatedRecipes.SingleOrDefault(
                        i => i.RatedById == ratedRecipe.RatedById && i.RecipeId == ratedRecipe.RecipeId);
                    var recipe = entities.Recipes.Single(i => i.Id == ratedRecipe.RecipeId && !i.IsDeleted);
                    if (exist != null) // The user has already rated the Recipe
                    {
                        recipe.TotalRates = recipe.TotalRates - exist.Rate + ratedRecipe.Rate;
                        exist.Rate = ratedRecipe.Rate;
                    }
                    else
                    {
                        // Init object to insert
                        RatedRecipe r = new RatedRecipe
                        {
                            RecipeId = ratedRecipe.RecipeId,
                            Rate = ratedRecipe.Rate,
                            RatedById = ratedRecipe.RatedById,
                            RatedDate = DateTime.Now
                        };
                        entities.RatedRecipes.Add(r);
                        recipe.RateTimes++;
                        recipe.TotalRates += ratedRecipe.Rate;
                    }

                    entities.SaveChanges();

                    return new Feedback<Recipe>(true, null, new Recipe { RateTimes = recipe.RateTimes, TotalRates = recipe.TotalRates });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Recipe>(false, Messages.GeneralError);
            }
        }

        public Feedback<Recipe> UpdateRecipe(Recipe q)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var recipe = entities.Recipes.SingleOrDefault(i => i.Id == q.Id && !q.IsDeleted);

                    if (recipe == null)
                        return new Feedback<Recipe>(false, Messages.RecipeNotExisted);

                    recipe.Title = q.Title;
                    recipe.ImageUrl = q.ImageUrl;
                    recipe.ContentHtml = q.ContentHtml;
                    recipe.ContentText = q.ContentText;
                    recipe.MappingCategoryId = q.MappingCategoryId;
                    recipe.UpdatedById = q.UpdatedById;
                    recipe.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<Recipe>(true, null, recipe);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Recipe>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportRecipe(ReportedRecipe rt)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedRecipes.Any(t => t.RecipeId == rt.RecipeId && !t.IsIgnored && t.ReportedById == rt.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRq = new ReportedRecipe
                    {
                        IsIgnored = false,
                        RecipeId = rt.RecipeId,
                        Reason = rt.Reason,
                        ReportedById = rt.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedRecipes.Add(newRq);

                    var q = entities.Recipes.Single(i => i.Id == rt.RecipeId);
                    q.Reports++;

                    entities.SaveChanges();

                    return new Feedback(true, string.Empty);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.GeneralError);
            }
        }

        public Feedback AddToFavorite(FavoriteRecipe favoriteRecipe)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var exist = entities.FavoriteRecipes.SingleOrDefault(
                            i => i.UserId == favoriteRecipe.UserId && i.RecipeId == favoriteRecipe.RecipeId);
                    if (exist == null)
                    {
                        FavoriteRecipe fr = new FavoriteRecipe
                        {
                            RecipeId = favoriteRecipe.RecipeId,
                            UserId = favoriteRecipe.UserId,
                            AddedDate = DateTime.Now
                        };
                        entities.FavoriteRecipes.Add(fr);
                        entities.SaveChanges();
                    }

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Search Recipes

        public SearchRecipeFeedback SearchRecipes(SearchCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.Recipes
                                join u in entities.Users on n.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on n.MappingCategoryId equals m.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                where !n.IsDeleted && !s.IsDeleted && !c.IsDeleted && n.TitleSearch.Contains(criteria.TitleSearch)
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.TotalRates,
                                    n.RateTimes,
                                    n.IsApproved,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchRecipeFeedback feedback = new SearchRecipeFeedback(true);

                    // Get total recipe count
                    feedback.TotalRecipes = query.Count();

                    // Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                            break;
                        case Order.Rates:
                            query = query.OrderByDescending(i => i.RateTimes == 0 ? 0 : (i.TotalRates / i.RateTimes)).ThenByDescending(i => i.Views);
                            break;
                        case Order.Approval:
                            query = query.Where(i => i.IsApproved).OrderByDescending(i => i.Views);
                            break;
                    }

                    // Paging
                    query = query
                        .Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize)
                        .Take(criteria.PageInfo.PageSize);

                    // Construct news list from queryable list
                    IList<Recipe> recipes = new List<Recipe>();
                    feedback.Recipes = recipes;
                    foreach (var n in query)
                    {
                        recipes.Add(new Recipe
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
                            TotalRates = n.TotalRates,
                            RateTimes = n.RateTimes,
                            IsApproved = n.IsApproved,
                            Views = n.Views,
                            Comments = n.Comments,
                            ContentText = n.ContentText,
                            PostedBy = new User
                            {
                                Id = n.UserId,
                                Username = n.Username
                            },
                        });
                    }

                    return feedback;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new SearchRecipeFeedback(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Sort Recipes

        public SearchRecipeFeedback SortRecipes(SortCriteria criteria, int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.Recipes
                                join u in entities.Users on n.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on n.MappingCategoryId equals m.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                where !n.IsDeleted && !s.IsDeleted && !c.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.TotalRates,
                                    n.RateTimes,
                                    n.IsApproved,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchRecipeFeedback feedback = new SearchRecipeFeedback(true);

                    // Get total news count
                    feedback.TotalRecipes = query.Count();

                    // Sorting
                    if (criteria.Content == "recipes")
                    {
                        switch (criteria.OrderBy)
                        {
                            case Order.Views:
                                query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                                break;
                            case Order.Comments:
                                query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                                break;
                            case Order.Rates:
                                query =
                                    query.OrderByDescending(i => i.RateTimes == 0 ? 0 : (i.TotalRates / i.RateTimes)).ThenByDescending(
                                        i => i.Views);
                                break;
                            case Order.Recent:
                                query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                                break;
                            case Order.Approval:
                                query = query.Where(i => i.IsApproved).OrderByDescending(i => i.Views);
                                break;
                        }
                    }
                    else
                    {
                        query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                    }

                    //Take
                    query = query.Take(take);

                    // Construct recipe list from queryable list
                    IList<Recipe> recipes = new List<Recipe>();
                    feedback.Recipes = recipes;
                    foreach (var n in query)
                    {
                        recipes.Add(new Recipe
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
                            TotalRates = n.TotalRates,
                            RateTimes = n.RateTimes,
                            IsApproved = n.IsApproved,
                            Views = n.Views,
                            Comments = n.Comments,
                            ContentText = n.ContentText,
                            PostedBy = new User
                            {
                                Id = n.UserId,
                                Username = n.Username
                            },
                        });
                    }

                    return feedback;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new SearchRecipeFeedback(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Approve Recipes

        public ListRecipeFeedback GetAllUnapprovedRecipe(PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Recipes
                                join u in entities.Users on q.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                where !q.IsDeleted && !q.IsApproved && !c.IsDeleted && !s.IsDeleted
                                orderby q.PostedDate descending
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.ImageUrl,
                                    q.MappingCategoryId,
                                    q.PostedById,
                                    q.PostedDate,
                                    q.RateTimes,
                                    q.TotalRates,
                                    q.Views,
                                    q.Comments,
                                    q.ContentHtml,
                                    q.ContentText,
                                    q.IsApproved,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();

                    query = query.Skip((pageInfo.PageNo - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
                    var recipes = new List<Recipe>();
                    foreach (var q in query)
                    {
                        recipes.Add(new Recipe()
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            MappingCategories = new RecipeCategoryMapping { Id = q.MappingCategoryId },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            RateTimes = q.RateTimes,
                            TotalRates = q.TotalRates,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentHtml = q.ContentHtml,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListRecipeFeedback(true, null) { Recipes = recipes, Total = total };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public ListRecipeFeedback GetAllApprovedRecipe(PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Recipes
                                join u in entities.Users on q.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on q.MappingCategoryId equals m.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                where !q.IsDeleted && q.IsApproved && !c.IsDeleted && !s.IsDeleted
                                orderby q.PostedDate descending
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.ImageUrl,
                                    q.MappingCategoryId,
                                    q.PostedById,
                                    q.PostedDate,
                                    q.RateTimes,
                                    q.TotalRates,
                                    q.Views,
                                    q.Comments,
                                    q.ContentHtml,
                                    q.ContentText,
                                    q.IsApproved,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();

                    query = query.Skip((pageInfo.PageNo - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
                    var recipes = new List<Recipe>();
                    foreach (var q in query)
                    {
                        recipes.Add(new Recipe()
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            MappingCategories = new RecipeCategoryMapping { Id = q.MappingCategoryId },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            RateTimes = q.RateTimes,
                            TotalRates = q.TotalRates,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentHtml = q.ContentHtml,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListRecipeFeedback(true, null) { Recipes = recipes, Total = total };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback ApproveRecipe(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Recipe q = entities.Recipes.Single(i => i.Id == id);
                    q.IsApproved = true;
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

        public Feedback UnapproveRecipe(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Recipe q = entities.Recipes.Single(i => i.Id == id);
                    q.IsApproved = false;
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

        #endregion

        #region Top Recipes

        public Feedback<IList<Recipe>> GetTopRecipes(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from r in entities.Recipes
                                join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted
                                orderby r.PostedDate
                                select new
                                           {
                                               r.Id,
                                               r.Title,
                                               r.TitleUrl,
                                               r.PostedDate
                                           };

                    query = query.Take(take);

                    var recipes = new List<Recipe>();
                    foreach (var item in query)
                    {
                        recipes.Add(new Recipe { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, PostedDate = item.PostedDate });
                    }

                    return new Feedback<IList<Recipe>>(true, null, recipes);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Recipe>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<Recipe>> GetTopRecipesByCategory(int take, int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from r in entities.Recipes
                                join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted && r.MappingCategoryId == id
                                orderby r.PostedDate
                                select new
                                {
                                    r.Id,
                                    r.Title,
                                    r.TitleUrl,
                                    r.PostedDate,
                                    r.Views,
                                    r.ImageUrl
                                };
                    var recipes = new List<Recipe>();
                    foreach (var item in query)
                    {
                        recipes.Add(new Recipe { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, Views = item.Views, ImageUrl = item.ImageUrl });
                    }

                    return new Feedback<IList<Recipe>>(true, null, recipes);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Recipe>>(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}