using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Interfaces;
using CRS.Business.LevelManagement;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Common;
using CRS.Resources;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;

namespace CRS.Business.Repositories
{
    public class UserRepository : IUserRepository
    {

        #region Implementation of IUserRepository

        public Feedback<IList<UserState>> GetAllUserStates()
        {
            using (var entities = new CrsEntities())
            {
                try
                {
                    return new Feedback<IList<UserState>>(true, null, entities.UserStates.ToList());
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new Feedback<IList<UserState>>(false);
                }
            }
        }

        public Feedback<IList<User>> GetAllUsers()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.Users
                        .Where(u => !u.IsDeleted)
                        .Select(u => new
                                         {
                                             u.Id,
                                             u.Username,
                                             u.Email,
                                             u.AvatarUrl,
                                             u.Point,
                                             u.Level,
                                             u.LastLogin,
                                             u.CreatedDate,
                                             u.UserStateId
                                         })
                        .OrderByDescending(i => i.CreatedDate);

                    IList<User> users = new List<User>();
                    foreach (var q in queryable)
                    {
                        users.Add(new User
                        {
                            Id = q.Id,
                            Username = q.Username,
                            Email = q.Email,
                            AvatarUrl = q.AvatarUrl,
                            Point = q.Point,
                            Level = q.Level,
                            LastLogin = q.LastLogin,
                            CreatedDate = q.CreatedDate,
                            UserStateId = q.UserStateId
                        });
                    }

                    return new Feedback<IList<User>>(true, null, users);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<User>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<User>> GetSimilarUsers(string username)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (username == null)
                    {
                        var queryable = entities.Users
                            .Where(u => !u.IsDeleted)
                            .Select(u => new
                                             {
                                                 u.Id,
                                                 u.Username,
                                                 u.Email,
                                                 u.AvatarUrl,
                                                 u.Point,
                                                 u.Level,
                                                 u.LastLogin,
                                                 u.CreatedDate,
                                                 u.UserStateId
                                             })
                            .OrderByDescending(i => i.CreatedDate);

                        IList<User> users = new List<User>();
                        foreach (var q in queryable)
                        {
                            users.Add(new User
                            {
                                Id = q.Id,
                                Username = q.Username,
                                Email = q.Email,
                                AvatarUrl = q.AvatarUrl,
                                Point = q.Point,
                                Level = q.Level,
                                LastLogin = q.LastLogin,
                                CreatedDate = q.CreatedDate,
                                UserStateId = q.UserStateId
                            });
                        }

                        return new Feedback<IList<User>>(true, null, users);
                    }
                    else
                    {
                        var queryable = entities.Users
                           .Where(u => !u.IsDeleted && u.Username.Contains(username))
                           .Select(u => new
                                            {
                                                u.Id,
                                                u.Username,
                                                u.Email,
                                                u.AvatarUrl,
                                                u.Point,
                                                u.Level,
                                                u.LastLogin,
                                                u.CreatedDate,
                                                u.UserStateId
                                            })
                           .OrderByDescending(i => i.CreatedDate);

                        IList<User> users = new List<User>();
                        foreach (var q in queryable)
                        {
                            users.Add(new User
                            {
                                Id = q.Id,
                                Username = q.Username,
                                Email = q.Email,
                                AvatarUrl = q.AvatarUrl,
                                Point = q.Point,
                                Level = q.Level,
                                LastLogin = q.LastLogin,
                                CreatedDate = q.CreatedDate,
                                UserStateId = q.UserStateId
                            });
                        }

                        return new Feedback<IList<User>>(true, null, users);
                    }                   
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<User>>(false, Messages.GeneralError);
            }
        }

        public Feedback<User> DeleteUser(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Single(i => i.Id == id && !i.IsDeleted);

                    // Cannot delete admin
                    if (user.Email.Equals(AppConfigs.AdminEmail))
                    {
                        return new Feedback<User>(false, Messages.CannotDeleteAdminInfo);
                    }

                    user.IsDeleted = true;
                    user.LastUpdate = DateTime.Now;

                    //Remove user roles
                    foreach (var a in entities.UserRoles.Where(t => t.RoleId == id).ToList())
                        entities.UserRoles.Remove(a);

                    entities.SaveChanges();

                    return new Feedback<User>(true, null, user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false, Messages.GeneralError);
            }
        }

        public Feedback<User> GetUserDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Include("UserRoles").Include("UserState").SingleOrDefault(i => i.Id == id && !i.IsDeleted);
                    if (user == null)
                        return new Feedback<User>(false, Messages.GetUser_NotFound);

                    return new Feedback<User>(true, null, user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false, Messages.GeneralError);
            }
        }

        public Feedback<User> GetPersonalInfo(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var record = entities.Users
                        .Where(i => i.Id == id && !i.IsDeleted)
                        .Select(i => new
                                         {
                                             i.Username,
                                             i.Password,
                                             i.Email,
                                             i.AvatarUrl,
                                             i.Level,
                                             i.Sex,
                                             i.Birthday,
                                             i.Hobbies,
                                             i.CookingExp,
                                             i.LocationId,
                                             i.SocialNetworkUrl,
                                             i.CreatedDate,
                                             i.Point,
                                             i.TodayPoint
                                         })
                        .SingleOrDefault();

                    User user;
                    if (record.LocationId != null)
                    {
                        var locationRecord = entities.Locations.Where(i => i.Id == record.LocationId).Select(i => new { i.Name }).SingleOrDefault();

                        user = new User
                        {
                            Id = id,
                            Username = record.Username,
                            Email = record.Email,
                            Password = record.Password,
                            AvatarUrl = record.AvatarUrl,
                            Level = record.Level,
                            Sex = record.Sex,
                            Birthday = record.Birthday,
                            Hobbies = record.Hobbies,
                            CookingExp = record.CookingExp,
                            LocationId = record.LocationId,
                            SocialNetworkUrl = record.SocialNetworkUrl,
                            CreatedDate = record.CreatedDate,
                            Point = record.Point,
                            TodayPoint = record.TodayPoint,
                            Location = new Location { Id = record.LocationId.Value, Name = locationRecord.Name }
                        };
                    }
                    else
                    {
                        user = new User
                                        {
                                            Id = id,
                                            Username = record.Username,
                                            Email = record.Email,
                                            Password = record.Password,
                                            AvatarUrl = record.AvatarUrl,
                                            Level = record.Level,
                                            Sex = record.Sex,
                                            Birthday = record.Birthday,
                                            Hobbies = record.Hobbies,
                                            CookingExp = record.CookingExp,
                                            LocationId = record.LocationId,
                                            SocialNetworkUrl = record.SocialNetworkUrl,
                                            CreatedDate = record.CreatedDate,
                                            Point = record.Point,
                                            TodayPoint = record.TodayPoint
                                        };
                    }

                    user.NumberOfNews = entities.News.Count(i => i.PostedById == id && !i.IsDeleted);
                    user.NumberOfQuestions = entities.Questions.Count(i => i.PostedById == id && !i.IsDeleted);

                    user.NumberOfAnswers = (from c in entities.Answers
                                            join n in entities.Questions on c.QuestionId equals n.Id
                                            where !c.IsDeleted && !n.IsDeleted && c.PostedById == id
                                            select new { c.Id }).Count();

                    user.NumberOfTips = (from t in entities.Tips
                                         join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                         where !t.IsDeleted && !c.IsDeleted && t.PostedById == id
                                         select new { t.Id }).Count();

                    user.NumberOfRecipes = (from r in entities.Recipes
                                            join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                            join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                            join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                            where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted && r.PostedById == id
                                            select new { r.Id }).Count();

                    return new Feedback<User>(true, null, user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false, Messages.GeneralError);
            }
        }

        public Feedback<User> UpdateUser(User c, IList<int> roleIds)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Single(i => i.Id == c.Id);
                    user.Point = c.Point;
                    user.Level = LevelHandler.CurrentLevel(user.Point);
                    user.UserStateId = c.UserStateId;

                    // Cannot change admin email
                    if (user.Email.Equals(AppConfigs.AdminEmail))
                    {
                        return new Feedback<User>(false, Messages.CannotChangeAdminInfo);
                    }

                    //Remove from user roles
                    foreach (var a in entities.UserRoles.Where(t => t.UserId == c.Id))
                        entities.UserRoles.Remove(a);

                    //Add new roles for user
                    foreach (int itemId in roleIds)
                    {
                        UserRole userRole = new UserRole
                        {
                            UserId = c.Id,
                            RoleId = itemId
                        };
                        entities.UserRoles.Add(userRole);
                    }

                    entities.SaveChanges();

                    return new Feedback<User>(true, null, user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false, Messages.GeneralError);
            }
        }

        public Feedback<string> UpdateUserProfile(User currentUser)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Single(i => i.Id == currentUser.Id);
                    string oldAvatar = null;

                    user.Sex = currentUser.Sex;
                    user.Birthday = currentUser.Birthday;
                    user.Hobbies = currentUser.Hobbies;
                    user.CookingExp = currentUser.CookingExp;
                    user.SocialNetworkUrl = currentUser.SocialNetworkUrl;
                    user.LocationId = currentUser.LocationId;
                    if (currentUser.AvatarUrl != null)
                    {
                        oldAvatar = user.AvatarUrl;
                        user.AvatarUrl = currentUser.AvatarUrl;
                    }
                    user.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<string>(true, null, oldAvatar);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<string>(false, Messages.GeneralError);
            }
        }

        public Feedback<string> RemoveAvatar(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Single(i => i.Id == id);
                    string oldAvatar = user.AvatarUrl;
                    user.AvatarUrl = null;
                    user.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<string>(true, null, oldAvatar);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<string>(false, Messages.GeneralError);
            }
        }

        public Feedback<User> ViewOtherUserInformaton(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var record = entities.Users
                        .Where(i => i.Id == id && !i.IsDeleted)
                        .Select(i => new
                                         {
                                             i.Username,
                                             i.Sex,
                                             i.Birthday,
                                             i.Hobbies,
                                             i.CookingExp,
                                             i.SocialNetworkUrl,
                                             i.LocationId,
                                             i.AvatarUrl,
                                             i.CreatedDate,
                                             i.Point,
                                             i.Level
                                         })
                        .SingleOrDefault();

                    if (record == null)
                        return new Feedback<User>(false, Messages.GetUser_NotFound);

                    User user = new User
                                 {
                                     Id = id,
                                     Username = record.Username,
                                     Sex = record.Sex,
                                     Birthday = record.Birthday,
                                     Hobbies = record.Hobbies,
                                     CookingExp = record.CookingExp,
                                     SocialNetworkUrl = record.SocialNetworkUrl,
                                     LocationId = record.LocationId,
                                     AvatarUrl = record.AvatarUrl,
                                     CreatedDate = record.CreatedDate,
                                     Point = record.Point,
                                     Level = record.Level
                                 };

                    user.NumberOfQuestions = entities.Questions.Count(i => i.PostedById == id && !i.IsDeleted);
                    user.NumberOfNews = entities.News.Count(i => i.PostedById == id && !i.IsDeleted);

                    user.NumberOfAnswers = (from c in entities.Answers
                                            join n in entities.Questions on c.QuestionId equals n.Id
                                            where !c.IsDeleted && !n.IsDeleted && c.PostedById == id
                                            select new { c.Id }).Count();

                    user.NumberOfTips = (from t in entities.Tips
                                         join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                         where !t.IsDeleted && !c.IsDeleted && t.PostedById == id
                                         select new { t.Id }).Count();

                    user.NumberOfRecipes = (from r in entities.Recipes
                                            join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                            join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                            join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                            where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted && r.PostedById == id
                                            select new { r.Id }).Count();

                    return new Feedback<User>(true, null, user);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false);
            }
        }

        public Feedback EmailExists(string email)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User exists = entities.Users.FirstOrDefault(i => i.Email.Equals(email));
                    if (exists == null)
                        return new Feedback(false);

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.GeneralError);
            }
        }

        public ListQuestionFeedback GetUserQuestion(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    List<Question> questions = new List<Question>();

                    var query = from q in entities.Questions
                                join qa in
                                    (from a in entities.Answers
                                     where !a.IsDeleted
                                     group a by a.QuestionId
                                         into ga
                                         select new { QuestionId = ga.Key, Count = (int?)ga.Count() }) on q.Id equals
                                    qa.QuestionId into j1
                                from j2 in j1.DefaultIfEmpty()
                                where !q.IsDeleted && q.PostedById == userid
                                orderby q.PostedDate descending
                                select new
                                           {
                                               q.Id,
                                               q.Title,
                                               q.TitleUrl,
                                               q.PostedDate,
                                               q.Views,
                                               AnswerTimes = j2.Count ?? 0
                                           };

                    var total = query.Count();

                    foreach (var item in query)
                    {
                        questions.Add(new Question
                                          {
                                              Id = item.Id,
                                              Title = item.Title,
                                              TitleUrl = item.TitleUrl,
                                              PostedDate = item.PostedDate,
                                              Views = item.Views,
                                              AnswerTimes = item.AnswerTimes
                                          });
                    }

                    return new ListQuestionFeedback(true, null) { Questions = questions, Total = total };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListQuestionFeedback(false, Messages.GeneralError);
            }
        }

        public ListAnswerFeedback GetUserAnswer(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var answers = new List<Answer>();
                    var query = (from a in entities.Answers
                                 join u2 in entities.Users on a.PostedById equals u2.Id
                                 join q in entities.Questions on a.QuestionId equals q.Id
                                 orderby a.PostedDate descending
                                 where !a.IsDeleted && !q.IsDeleted && u2.Id == userid
                                 select new
                                 {
                                     a.Id,
                                     a.ContentText,
                                     a.PostedDate,
                                     a.DownVotes,
                                     a.UpVotes,
                                     AnswerId = a.QuestionId,
                                     AnswerTitle = q.Title,
                                     AnswerTitleUrl = q.TitleUrl
                                 });
                    var total = query.Count();

                    foreach (var item in query)
                    {
                        answers.Add(new Answer()
                        {
                            Id = item.Id,
                            ContentText = item.ContentText,
                            DownVotes = item.DownVotes,
                            PostedDate = item.PostedDate,
                            UpVotes = item.UpVotes,
                            Question = new Question { Id = item.AnswerId, Title = item.AnswerTitle, TitleUrl = item.AnswerTitleUrl }
                        });
                    }

                    return new ListAnswerFeedback(true, null) { Answers = answers, Total = total };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListAnswerFeedback(false, Messages.GeneralError);
            }
        }

        public ListTipFeedback GetUserTip(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from t in entities.Tips
                                join u in entities.Users on t.PostedById equals u.Id
                                join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                where !t.IsDeleted && !c.IsDeleted && u.Id == userid
                                orderby t.PostedDate descending
                                select new
                                           {
                                               t.Id,
                                               t.Title,
                                               t.TitleUrl,
                                               t.ImageUrl,
                                               t.TipCategoryId,
                                               c.Name,
                                               t.PostedById,
                                               t.PostedDate,
                                               t.UpVotes,
                                               t.DownVotes,
                                               t.Views,
                                               t.Comments,
                                               t.ContentText,
                                               UserId = u.Id,
                                               u.Username
                                           };
                    var total = query.Count();
                    var tips = new List<Tip>();
                    foreach (var q in query)
                    {
                        tips.Add(new Tip
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            TipCategories = new TipCategory { Id = q.TipCategoryId, Name = q.Name },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            UpVotes = q.UpVotes,
                            DownVotes = q.DownVotes,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListTipFeedback(true, null) { Tips = tips, Total = total };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListTipFeedback(false, Messages.GeneralError);
            }
        }

        public ListNewsFeedback GetUserNews(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from n in entities.News
                                join u in entities.Users on n.PostedById equals u.Id
                                where !n.IsDeleted && u.Id == userid
                                orderby n.PostedDate descending
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedById,
                                    n.PostedDate,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();
                    var news = new List<News>();
                    foreach (var q in query)
                    {
                        news.Add(new News
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListNewsFeedback(true, null) { News = news, Total = total };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListNewsFeedback(false, Messages.GeneralError);
            }
        }

        public ListRecipeFeedback GetUserRecipe(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from t in entities.Recipes
                                join u in entities.Users on t.PostedById equals u.Id
                                join c in entities.RecipeCategoryMappings on t.MappingCategoryId equals c.Id
                                where !t.IsDeleted && u.Id == userid
                                orderby t.PostedDate descending
                                select new
                                {
                                    t.Id,
                                    t.Title,
                                    t.TitleUrl,
                                    t.ImageUrl,
                                    t.MappingCategoryId,
                                    t.PostedById,
                                    t.PostedDate,
                                    t.TotalRates,
                                    t.RateTimes,
                                    t.IsApproved,
                                    t.Views,
                                    t.Comments,
                                    t.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();
                    var recipes = new List<Recipe>();
                    foreach (var q in query)
                    {
                        recipes.Add(new Recipe
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            MappingCategories = new RecipeCategoryMapping { Id = q.MappingCategoryId },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            TotalRates = q.TotalRates,
                            RateTimes = q.RateTimes,
                            IsApproved = q.IsApproved,
                            Views = q.Views,
                            Comments = q.Comments,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListRecipeFeedback(true, null) { Recipes = recipes, Total = total };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListRecipeFeedback(false, Messages.GeneralError);
            }
        }

        public ListRecipeFeedback GetUserFavoriteRecipes(int userid)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from f in entities.FavoriteRecipes
                                join r in entities.Recipes on f.RecipeId equals r.Id
                                join u in entities.Users on r.PostedById equals u.Id
                                join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                join s in entities.RecipeCategories on m.RecipeSmallCategoryId equals s.Id
                                where f.UserId == userid && !r.IsDeleted && !c.IsDeleted && !s.IsDeleted
                                select new
                                {
                                    r.Id,
                                    r.Title,
                                    r.TitleUrl,
                                    r.ImageUrl,
                                    r.MappingCategoryId,
                                    r.PostedById,
                                    r.PostedDate,
                                    r.RateTimes,
                                    r.TotalRates,
                                    r.Views,
                                    r.Comments,
                                    r.ContentHtml,
                                    r.ContentText,
                                    r.IsApproved,
                                    UserId = u.Id,
                                    u.Username
                                };

                    var total = query.Count();
                    var recipes = new List<Recipe>();
                    foreach (var q in query)
                    {
                        recipes.Add(new Recipe
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            ImageUrl = q.ImageUrl,
                            MappingCategories = new RecipeCategoryMapping { Id = q.MappingCategoryId },
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            TotalRates = q.TotalRates,
                            RateTimes = q.RateTimes,
                            IsApproved = q.IsApproved,
                            Views = q.Views,
                            Comments = q.Comments,
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

        public Feedback DeleteUserFavoriteRecipes(List<int> idList, int userId)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var currentFavorites = entities.FavoriteRecipes.Where(i => i.UserId == userId).ToList();
                    foreach (var id in idList)
                    {
                        var exist = currentFavorites.SingleOrDefault(i => i.RecipeId == id);
                        if (exist != null)
                            entities.FavoriteRecipes.Remove(exist);
                    }

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

        #region top contributors
        public Feedback<IList<User>> GetTopContributors(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = entities.Users
                        .Select(i => new { i.Id, i.Username, i.Point, i.AvatarUrl })
                        .OrderByDescending(i => i.Point)
                        .Take(take);

                    var users = new List<User>();
                    foreach (var item in query)
                    {
                        users.Add(new User { Id = item.Id, Username = item.Username, Point = item.Point, AvatarUrl = item.AvatarUrl });
                    }

                    return new Feedback<IList<User>>(true, null, users);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<User>>(false, Messages.GeneralError);
            }
        }
        #endregion
    }
}