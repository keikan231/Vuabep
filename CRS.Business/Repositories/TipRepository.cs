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
    public class TipRepository : ITipRepository
    {
        #region Reported Tips

        public Feedback<IList<Tip>> GetAllReportedTips(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from q in entities.Tips
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
                    List<Tip> rq = new List<Tip>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new Tip
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

                    return new Feedback<IList<Tip>>(true, null, rq);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Tip>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<ReportedTip>> GetReportedTipDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedTips.Where(i => i.TipId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedTip> rq = new List<ReportedTip>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new ReportedTip
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
                    return new Feedback<IList<ReportedTip>>(true, null, rq);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedTip>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedTip(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get Tip needed to be deleted
                    Tip q = entities.Tips.Single(i => i.Id == id);
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
                    List<ReportedTip> rq = entities.ReportedTips.Where(i => i.TipId == id).ToList();
                    foreach (ReportedTip i in rq)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    Tip q = entities.Tips.Single(i => i.Id == id);
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

        #region Tips

        public ListTipFeedback GetAllTip(SortCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Tips
                                join u in entities.Users on q.PostedById equals u.Id
                                join c in entities.TipCategories on q.TipCategoryId equals c.Id
                                where !q.IsDeleted && !c.IsDeleted
                                orderby q.PostedDate descending
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.ImageUrl,
                                    q.TipCategoryId,
                                    c.Name,
                                    q.PostedById,
                                    q.PostedDate,
                                    q.UpVotes,
                                    q.DownVotes,
                                    q.Views,
                                    q.Comments,
                                    q.ContentHtml,
                                    q.ContentText,
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
                        case Order.Votes:
                            query =
                                query.OrderByDescending(i => (i.UpVotes - i.DownVotes)).ThenByDescending(
                                    i => i.Views);
                            break;
                        case Order.Recent:
                            query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                            break;
                    }

                    query = query.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);
                    var tips = new List<Tip>();
                    foreach (var q in query)
                    {
                        tips.Add(new Tip()
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
                            ContentHtml = q.ContentHtml,
                            ContentText = q.ContentText,
                        });
                    }

                    return new ListTipFeedback(true, null) { Tips = tips, Total = total };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListTipFeedback(false, Messages.GeneralError);
            }
        }

        public ListTipFeedback GetTipByCatogory(int tipCategoryId, SortCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Tips
                                join u in entities.Users on q.PostedById equals u.Id
                                join c in entities.TipCategories on q.TipCategoryId equals c.Id
                                where !q.IsDeleted && !c.IsDeleted && q.TipCategoryId == tipCategoryId
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.ImageUrl,
                                    q.TipCategoryId,
                                    c.Name,
                                    q.PostedById,
                                    q.PostedDate,
                                    q.UpVotes,
                                    q.DownVotes,
                                    q.Views,
                                    q.Comments,
                                    q.ContentHtml,
                                    q.ContentText,
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
                        case Order.Votes:
                            query =
                                query.OrderByDescending(i => (i.UpVotes - i.DownVotes)).ThenByDescending(
                                    i => i.Views);
                            break;
                        case Order.Recent:
                            query = query.OrderByDescending(i => i.PostedDate).ThenByDescending(i => i.Views);
                            break;
                    }

                    query = query.Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize).Take(criteria.PageInfo.PageSize);
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
                            ContentHtml = q.ContentHtml,
                            ContentText = q.ContentText,
                        });
                    }

                    string tipCategoryName = "Mẹo vặt";
                    try
                    {
                        tipCategoryName =
                            entities.TipCategories.FirstOrDefault(i => i.Id == tipCategoryId && !i.IsDeleted).Name;
                    }
                    catch (Exception e)
                    {
                        return new ListTipFeedback(false, Messages.CategoryNotExisted);
                    }

                    return new ListTipFeedback(true, null) { Tips = tips, Total = total, TipCategoryName = tipCategoryName };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListTipFeedback(false, Messages.GeneralError);
            }
        }

        public InsertTipFeedback InsertTip(Tip q)
        {
            try
            {
                var tip = new Tip
                {
                    Title = q.Title,
                    TitleSearch = q.Title.ToSearchFriendly(),
                    TitleUrl = q.TitleUrl,
                    ImageUrl = q.ImageUrl,
                    ContentHtml = q.ContentHtml,
                    ContentText = q.ContentText,
                    TipCategoryId = q.TipCategoryId,
                    UpVotes = 0,
                    DownVotes = 0,
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
                    entities.Tips.Add(tip);
                    entities.SaveChanges();

                    var user = entities.Users.Single(t => t.Id == tip.PostedById);
                    if (user.TodayDate != DateTime.Today)
                    {
                        user.TodayDate = DateTime.Today;
                        user.TodayPoint = 0;
                    }

                    var createTipPoint = ReferenceDataCache.PointConfigCollection.CreateTip;
                    int maxPointPerDay = ReferenceDataCache.PointConfigCollection.MaxPointPerDay;

                    if (user.TodayPoint < maxPointPerDay)
                    {
                        if (user.TodayPoint + createTipPoint > maxPointPerDay)
                        {
                            user.Point += maxPointPerDay - user.TodayPoint.Value;
                            user.TodayPoint = maxPointPerDay;
                        }
                        else
                        {
                            user.Point += createTipPoint;
                            user.TodayPoint += createTipPoint;
                        }
                    }

                    user.Level = LevelHandler.CurrentLevel(user.Point);

                    //Update data
                    entities.SaveChanges();

                    return new InsertTipFeedback(true, Messages.InsertTipSuccess) { Tip = tip, NewPoint = user.Point };
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new InsertTipFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteTip(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Tip q = entities.Tips.Single(i => i.Id == id);
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

        public Feedback<Tip> GetTipForEditing(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var tip = entities.Tips.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    return tip != null ? new Feedback<Tip>(true, null, tip) : new Feedback<Tip>(false, Messages.TipNotExisted);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Tip>(false, Messages.GeneralError);
            }
        }

        public Feedback<Tip> GetTipDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from q in entities.Tips
                                 join u1 in entities.Users on q.PostedById equals u1.Id
                                 join c in entities.TipCategories on q.TipCategoryId equals c.Id
                                 where q.Id == id && !q.IsDeleted && !c.IsDeleted
                                 select new
                                 {
                                     TipId = q.Id,
                                     q.Title,
                                     q.TitleUrl,
                                     TipImageUrl = q.ImageUrl,
                                     TipPostedDate = q.PostedDate,
                                     TipPostedById = q.PostedById,
                                     TipContentHtml = q.ContentHtml,
                                     TipContentText = q.ContentText,
                                     TipCom = q.Comments,
                                     TipViews = q.Views,
                                     TipCat = q.TipCategoryId,
                                     TipCategoryName = c.Name,
                                     TipUsername = u1.Username,
                                     TipAvatarUrl = u1.AvatarUrl,
                                     TipLevel = u1.Level,
                                     TipUpVotes = q.UpVotes,
                                     TipDownVotes = q.DownVotes,
                                 }).ToList();
                    if (query.Count <= 0) return new Feedback<Tip>(false, Messages.TipNotExisted);

                    var tip = new Tip
                    {
                        Id = query[0].TipId,
                        Title = query[0].Title,
                        TitleUrl = query[0].TitleUrl,
                        ImageUrl = query[0].TipImageUrl,
                        PostedById = query[0].TipPostedById,
                        PostedDate = query[0].TipPostedDate,
                        PostedBy = new User { Id = query[0].TipPostedById, Username = query[0].TipUsername, AvatarUrl = query[0].TipAvatarUrl, Level = query[0].TipLevel },
                        TipCategories = new TipCategory { Id = query[0].TipCat, Name = query[0].TipCategoryName },
                        ContentHtml = query[0].TipContentHtml,
                        ContentText = query[0].TipContentText,
                        TipCategoryId = query[0].TipCat,
                        Comments = query[0].TipCom,
                        Views = query[0].TipViews,
                        UpVotes = query[0].TipUpVotes,
                        DownVotes = query[0].TipDownVotes
                    };

                    var tmpQues = entities.Tips.Single(i => i.Id == id);
                    tmpQues.Views++;
                    entities.SaveChanges();

                    return new Feedback<Tip>(true, null, tip);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Tip>(false, Messages.GeneralError);
            }
        }

        public Feedback<Tip> UpdateTip(Tip q)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var tip = entities.Tips.SingleOrDefault(i => i.Id == q.Id && !q.IsDeleted);

                    if (tip == null)
                        return new Feedback<Tip>(false, Messages.TipNotExisted);

                    tip.Title = q.Title;
                    tip.ImageUrl = q.ImageUrl;
                    tip.ContentHtml = q.ContentHtml;
                    tip.ContentText = q.ContentText;
                    tip.TipCategoryId = q.TipCategoryId;
                    tip.UpdatedById = q.UpdatedById;
                    tip.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<Tip>(true, null, tip);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Tip>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportTip(ReportedTip rt)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedTips.Any(t => t.TipId == rt.TipId && !t.IsIgnored && t.ReportedById == rt.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRq = new ReportedTip
                    {
                        IsIgnored = false,
                        TipId = rt.TipId,
                        Reason = rt.Reason,
                        ReportedById = rt.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedTips.Add(newRq);

                    var q = entities.Tips.Single(i => i.Id == rt.TipId);
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

        public Feedback<Tip> VoteTip(VotedTip vt)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var q = entities.Tips.Single(i => i.Id == vt.TipId);
                    var newVq = entities.VotedTips.SingleOrDefault(t => t.TipId == vt.TipId && t.VotedById == vt.VotedById);

                    //Do not allow seft voting
                    if (q.PostedById == vt.VotedById)
                    {
                        return new Feedback<Tip>(false, Messages.VoteContent_SeftVoting);
                    }

                    //Already voted with the same choice
                    if (newVq != null && newVq.IsUpVote == vt.IsUpVote)
                    {
                        return new Feedback<Tip>(true, null, new Tip { DownVotes = -1, UpVotes = -1 });
                    }

                    //Revote with other choice (Update)
                    if (newVq != null)
                    {
                        newVq.VotedDate = DateTime.Now;
                        newVq.IsUpVote = vt.IsUpVote;

                        if (vt.IsUpVote)
                            q.DownVotes--;
                        else
                            q.UpVotes--;
                    }
                    //Not vote yet (Insert)
                    else
                    {
                        newVq = new VotedTip
                        {
                            Id = 0,
                            IsUpVote = vt.IsUpVote,
                            TipId = vt.TipId,
                            VotedById = vt.VotedById,
                            VotedDate = DateTime.Now,
                        };
                        entities.VotedTips.Add(newVq);
                    }
                    if (vt.IsUpVote)
                        q.UpVotes++;
                    else
                        q.DownVotes++;


                    entities.SaveChanges();

                    return new Feedback<Tip>(true, null, q);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Tip>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Top Tips

        public Feedback<IList<Tip>> GetTopTips(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from t in entities.Tips
                                join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                where !t.IsDeleted && !c.IsDeleted
                                orderby t.PostedDate
                                select new
                                           {
                                               t.Id,
                                               t.Title,
                                               t.TitleUrl,
                                               t.PostedDate
                                           };

                    query = query.Take(take);
                    var tips = new List<Tip>();
                    foreach (var item in query)
                    {
                        tips.Add(new Tip { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, PostedDate = item.PostedDate });
                    }

                    return new Feedback<IList<Tip>>(true, null, tips);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Tip>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<Tip>> GetTopTipsByCategory(int take, int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from t in entities.Tips
                                join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                where !t.IsDeleted && !c.IsDeleted && t.TipCategoryId == id
                                orderby t.PostedDate
                                select new
                                {
                                    t.Id,
                                    t.Title,
                                    t.TitleUrl,
                                    t.PostedDate,
                                    t.Views,
                                    t.ImageUrl
                                };

                    query = query.Take(take);
                    var tips = new List<Tip>();
                    foreach (var item in query)
                    {
                        tips.Add(new Tip { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, Views = item.Views, ImageUrl = item.ImageUrl, PostedDate = item.PostedDate });
                    }

                    return new Feedback<IList<Tip>>(true, null, tips);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Tip>>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Search Tips

        public SearchTipFeedback SearchTips(SearchCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.Tips
                                join u in entities.Users on n.PostedById equals u.Id
                                join c in entities.TipCategories on n.TipCategoryId equals c.Id
                                where n.TitleSearch.Contains(criteria.TitleSearch)
                                    && !n.IsDeleted && !c.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.UpVotes,
                                    n.DownVotes,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchTipFeedback feedback = new SearchTipFeedback(true);

                    // Get total news count
                    feedback.TotalTips = query.Count();

                    // Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                            break;
                        case Order.Votes:
                            query = query.OrderByDescending(i => (i.UpVotes - i.DownVotes)).ThenByDescending(i => i.Views);
                            break;
                    }

                    // Paging
                    query = query
                        .Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize)
                        .Take(criteria.PageInfo.PageSize);

                    // Construct news list from queryable list
                    IList<Tip> tips = new List<Tip>();
                    feedback.Tips = tips;
                    foreach (var n in query)
                    {
                        tips.Add(new Tip
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
                            UpVotes = n.UpVotes,
                            DownVotes = n.DownVotes,
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
                return new SearchTipFeedback(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Sort Tips

        public SearchTipFeedback SortTips(SortCriteria criteria, int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = from n in entities.Tips
                                join u in entities.Users on n.PostedById equals u.Id
                                join c in entities.TipCategories on n.TipCategoryId equals c.Id
                                where !n.IsDeleted && !c.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.UpVotes,
                                    n.DownVotes,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u.Id,
                                    u.Username
                                };
                    SearchTipFeedback feedback = new SearchTipFeedback(true);

                    // Get total news count
                    feedback.TotalTips = query.Count();

                    // Sorting
                    if (criteria.Content == "tips")
                    {
                        switch (criteria.OrderBy)
                        {
                            case Order.Views:
                                query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                                break;
                            case Order.Comments:
                                query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                                break;
                            case Order.Votes:
                                query =
                                    query.OrderByDescending(i => (i.UpVotes - i.DownVotes)).ThenByDescending(
                                        i => i.Views);
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

                    //Take
                    query = query.Take(take);

                    // Construct news list from queryable list
                    IList<Tip> tips = new List<Tip>();
                    feedback.Tips = tips;
                    foreach (var n in query)
                    {
                        tips.Add(new Tip
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
                            UpVotes = n.UpVotes,
                            DownVotes = n.DownVotes,
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
                return new SearchTipFeedback(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}