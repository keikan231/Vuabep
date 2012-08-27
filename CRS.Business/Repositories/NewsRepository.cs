using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.LevelManagement;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common.Helpers;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class NewsRepository : INewsRepository
    {
        #region Reported News

        public Feedback<IList<News>> GetAllReportedNews(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from q in entities.News
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
                    List<News> rq = new List<News>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new News
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

                    return new Feedback<IList<News>>(true, null, rq);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<News>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<ReportedNews>> GetReportedNewsDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedNews.Where(i => i.NewsId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedNews> rq = new List<ReportedNews>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new ReportedNews
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
                    return new Feedback<IList<ReportedNews>>(true, null, rq);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedNews>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedNews(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get News needed to be deleted
                    News q = entities.News.Single(i => i.Id == id);
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
                    List<ReportedNews> rq = entities.ReportedNews.Where(i => i.NewsId == id).ToList();
                    foreach (ReportedNews i in rq)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    News q = entities.News.Single(i => i.Id == id);
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

        #region News

        public ListNewsFeedback GetAllNews(SortCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from n in entities.News
                                join u in entities.Users on n.PostedById equals u.Id
                                where !n.IsDeleted && !u.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();
                    // Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Views);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Comments);
                            break;
                        case Order.Recent:
                            query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                            break;
                    }

                    query = query.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);
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
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListNewsFeedback(false, Messages.GeneralError);
            }
        }

        public InsertNewsFeedback InsertNews(News q)
        {
            try
            {
                var news = new News
                {
                    Title = q.Title,
                    TitleUrl = q.TitleUrl,
                    TitleSearch = q.Title.ToSearchFriendly(),
                    ImageUrl = q.ImageUrl,
                    ContentHtml = q.ContentHtml,
                    ContentText = q.ContentText,
                    Views = 0,
                    Comments = 0,
                    PostedById = q.PostedById,
                    PostedDate = DateTime.Now,
                    IsDeleted = false,
                    UpdatedById = null,
                    LastUpdate = null
                };
                using (var entities = new CrsEntities())
                {
                    // Add to DB
                    entities.News.Add(news);
                    entities.SaveChanges();

                    var user = entities.Users.Single(t => t.Id == news.PostedById);
                    if (user.TodayDate != DateTime.Today)
                    {
                        user.TodayDate = DateTime.Today;
                        user.TodayPoint = 0;
                    }

                    var createNewsPoint = ReferenceDataCache.PointConfigCollection.CreateNews;
                    int maxPointPerDay = ReferenceDataCache.PointConfigCollection.MaxPointPerDay;

                    if (user.TodayPoint < maxPointPerDay)
                    {
                        if (user.TodayPoint + createNewsPoint > maxPointPerDay)
                        {
                            user.Point += maxPointPerDay - user.TodayPoint.Value;
                            user.TodayPoint = maxPointPerDay;
                        }
                        else
                        {
                            user.Point += createNewsPoint;
                            user.TodayPoint += createNewsPoint;
                        }
                    }

                    user.Level = LevelHandler.CurrentLevel(user.Point);

                    //Update data
                    entities.SaveChanges();

                    return new InsertNewsFeedback(true, Messages.InsertNewsSuccess) { News = news, NewPoint = user.Point };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new InsertNewsFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteNews(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    News q = entities.News.Single(i => i.Id == id);
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

        public Feedback<News> GetNewsForEditing(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var news = entities.News.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    return news != null ? new Feedback<News>(true, null, news) : new Feedback<News>(false, Messages.NewsNotExisted);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<News>(false, Messages.GeneralError);
            }
        }

        public Feedback<News> GetNewsDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from q in entities.News
                                 join u1 in entities.Users on q.PostedById equals u1.Id
                                 where q.Id == id && !q.IsDeleted
                                 select new
                                 {
                                     NewsId = q.Id,
                                     q.Title,
                                     q.TitleUrl,
                                     NewsImageUrl = q.ImageUrl,
                                     NewsPostedDate = q.PostedDate,
                                     NewsPostedById = q.PostedById,
                                     NewsContentHtml = q.ContentHtml,
                                     NewsContentText = q.ContentText,
                                     NewsCom = q.Comments,
                                     NewsViews = q.Views,
                                     NewsUsername = u1.Username,
                                     NewsAvatarUrl = u1.AvatarUrl,
                                     NewsLevel = u1.Level
                                 }).ToList();
                    if (query.Count <= 0) return new Feedback<News>(false, Messages.NewsNotExisted);

                    var news = new News
                    {
                        Id = query[0].NewsId,
                        Title = query[0].Title,
                        TitleUrl = query[0].TitleUrl,
                        ImageUrl = query[0].NewsImageUrl,
                        PostedById = query[0].NewsPostedById,
                        PostedDate = query[0].NewsPostedDate,
                        PostedBy = new User { Id = query[0].NewsPostedById, Username = query[0].NewsUsername, AvatarUrl = query[0].NewsAvatarUrl, Level = query[0].NewsLevel },
                        ContentHtml = query[0].NewsContentHtml,
                        ContentText = query[0].NewsContentText,
                        Comments = query[0].NewsCom,
                        Views = query[0].NewsViews,
                    };

                    var tmpQues = entities.News.Single(i => i.Id == id);
                    tmpQues.Views++;
                    entities.SaveChanges();

                    return new Feedback<News>(true, null, news);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<News>(false, Messages.GeneralError);
            }
        }

        public Feedback<News> UpdateNews(News q)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var news = entities.News.SingleOrDefault(i => i.Id == q.Id && !q.IsDeleted);

                    if (news == null)
                        return new Feedback<News>(false, Messages.NewsNotExisted);

                    news.Title = q.Title;
                    news.ImageUrl = q.ImageUrl;
                    news.ContentHtml = q.ContentHtml;
                    news.ContentText = q.ContentText;
                    news.UpdatedById = q.UpdatedById;
                    news.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<News>(true, null, news);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<News>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportNews(ReportedNews rq)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedNews.Any(t => t.NewsId == rq.NewsId && !t.IsIgnored && t.ReportedById == rq.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRq = new ReportedNews
                    {
                        IsIgnored = false,
                        NewsId = rq.NewsId,
                        Reason = rq.Reason,
                        ReportedById = rq.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedNews.Add(newRq);

                    var q = entities.News.Single(i => i.Id == rq.NewsId);
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

        #endregion

        #region Top News

        public Feedback<IList<News>> GetTopNews(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = entities.News
                        .Where(i => !i.IsDeleted)
                        .OrderByDescending(i => i.PostedDate)
                        .Select(i => new { i.Id, i.Title, i.TitleUrl, i.PostedDate })
                        .Take(take);
                    var news = new List<News>();
                    foreach (var item in query)
                    {
                        news.Add(new News { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, PostedDate = item.PostedDate });
                    }

                    return new Feedback<IList<News>>(true, null, news);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<News>>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Search News

        public SearchNewsFeedback SearchNews(SearchCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.News
                                join u in entities.Users on n.PostedById equals u.Id
                                where n.TitleSearch.Contains(criteria.TitleSearch)
                                    && !n.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchNewsFeedback feedback = new SearchNewsFeedback(true);

                    // Get total news count
                    feedback.TotalNews = query.Count();

                    // Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Views);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Comments);
                            break;
                    }

                    // Paging
                    query = query
                        .Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize)
                        .Take(criteria.PageInfo.PageSize);

                    // Construct news list from queryable list
                    IList<News> news = new List<News>();
                    feedback.News = news;
                    foreach (var n in query)
                    {
                        news.Add(new News
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
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
                return new SearchNewsFeedback(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Sort News

        public SearchNewsFeedback SortNews(SortCriteria criteria, int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.News
                                join u in entities.Users on n.PostedById equals u.Id
                                where !n.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchNewsFeedback feedback = new SearchNewsFeedback(true);

                    // Get total news count
                    feedback.TotalNews = query.Count();

                    // Sorting
                    if (criteria.Content == "news")
                    {
                        switch (criteria.OrderBy)
                        {
                            case Order.Views:
                                query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Views);
                                break;
                            case Order.Comments:
                                query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Comments);
                                break;
                            case Order.Recent:
                                query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                                break;
                        }
                    }
                    else
                    {
                        query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                    }

                    query = query.Take(take);

                    // Construct news list from queryable list
                    IList<News> news = new List<News>();
                    feedback.News = news;
                    foreach (var n in query)
                    {
                        news.Add(new News
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
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
                return new SearchNewsFeedback(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}