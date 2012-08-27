using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class NewsCommentRepository : INewsCommentRepository
    {
        #region Manage reported News Comments

        public Feedback<IList<NewsComment>> GetAllReportedNewsComments(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from c in entities.NewsComments
                                    from u in entities.Users
                                    from p in entities.News
                                    where c.PostedById == u.Id && c.NewsId == p.Id && c.Reports >= minReportNumber && !c.IsDeleted
                                    orderby c.Reports descending
                                    select new
                                    {
                                        c.Id,
                                        c.ContentText,
                                        c.Reports,
                                        c.PostedDate,
                                        c.NewsId,
                                        c.PostedById,
                                        p.Title,
                                        p.TitleUrl,
                                        u.Username
                                    };
                    List<NewsComment> reportedNewsComments = new List<NewsComment>();
                    foreach (var q in queryable)
                    {
                        reportedNewsComments.Add(new NewsComment
                        {
                            Id = q.Id,
                            ContentText = q.ContentText,
                            PostedDate = q.PostedDate,
                            Reports = q.Reports,
                            PostedBy = new User
                            {
                                Id = q.PostedById,
                                Username = q.Username
                            },
                            News = new News
                            {
                                Id = q.NewsId,
                                TitleUrl = q.TitleUrl,
                                Title = q.Title,
                            }
                        });
                    }

                    return new Feedback<IList<NewsComment>>(true, null, reportedNewsComments);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<NewsComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedComment(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get comment needed to be deleted
                    var c = entities.NewsComments.Single(i => i.Id == id);
                    c.IsDeleted = true;

                    var news = entities.News.Single(t => t.Id == c.NewsId);
                    var count = entities.NewsComments.Where(t => !t.IsDeleted && t.NewsId == c.NewsId).Count();
                    news.Comments = count - 1;

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
                    // Delete attached reports of the comment
                    List<ReportedNewsComment> rc = entities.ReportedNewsComments.Where(i => i.CommentId == id).ToList();
                    foreach (ReportedNewsComment i in rc)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    NewsComment c = entities.NewsComments.Single(i => i.Id == id);
                    c.Reports = 0;
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

        public Feedback<IList<ReportedNewsComment>> GetReportedCommentDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedNewsComments.Where(i => i.CommentId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedNewsComment> reportedNewsComments = new List<ReportedNewsComment>();
                    foreach (var q in queryable)
                    {
                        reportedNewsComments.Add(new ReportedNewsComment
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
                    return new Feedback<IList<ReportedNewsComment>>(true, null, reportedNewsComments);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedNewsComment>>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region News Comments

        public Feedback<IList<NewsComment>> GetNewsComments(int newsId, PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from c in entities.NewsComments
                                from u in entities.Users
                                where c.NewsId == newsId && c.PostedById == u.Id && !c.IsDeleted
                                orderby c.PostedDate descending
                                select new
                                {
                                    c.Id,
                                    c.ContentText,
                                    c.UpVotes,
                                    c.DownVotes,
                                    c.PostedDate,
                                    u.Username,
                                    UserId = u.Id,
                                    u.AvatarUrl
                                };
                    // Paging
                    query = query
                        .Skip((pageInfo.PageNo - 1) * pageInfo.PageSize)
                        .Take(pageInfo.PageSize);

                    // Construct comment objects from query
                    IList<NewsComment> newsComments = new List<NewsComment>();
                    foreach (var record in query)
                    {
                        newsComments.Add(new NewsComment
                        {
                            Id = record.Id,
                            ContentText = record.ContentText,
                            UpVotes = record.UpVotes,
                            DownVotes = record.DownVotes,
                            PostedDate = record.PostedDate,
                            PostedById = record.UserId,
                            PostedBy = new User
                            {
                                Id = record.UserId,
                                Username = record.Username,
                                AvatarUrl = record.AvatarUrl
                            }
                        });
                    }

                    return new Feedback<IList<NewsComment>>(true, null, newsComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<NewsComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<NewsComment>> GetHighlightNewsComments(int newsId, int number)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from c in entities.NewsComments
                                 from u in entities.Users
                                 where c.NewsId == newsId && c.PostedById == u.Id && !c.IsDeleted && c.UpVotes - c.DownVotes > 0
                                 orderby c.UpVotes - c.DownVotes descending
                                 select new
                                 {
                                     c.Id,
                                     c.ContentText,
                                     c.UpVotes,
                                     c.DownVotes,
                                     c.PostedDate,
                                     u.Username,
                                     UserId = u.Id,
                                     u.AvatarUrl
                                 }).Take(number);

                    // Construct comment objects from query
                    IList<NewsComment> newsComments = new List<NewsComment>();
                    foreach (var record in query)
                    {
                        newsComments.Add(new NewsComment
                        {
                            Id = record.Id,
                            ContentText = record.ContentText,
                            UpVotes = record.UpVotes,
                            DownVotes = record.DownVotes,
                            PostedDate = record.PostedDate,
                            PostedById = record.UserId,
                            PostedBy = new User
                            {
                                Id = record.UserId,
                                Username = record.Username,
                                AvatarUrl = record.AvatarUrl
                            }
                        });
                    }

                    return new Feedback<IList<NewsComment>>(true, null, newsComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<NewsComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<NewsComment> InsertNewsComment(NewsComment c)
        {
            var nComment = new NewsComment
            {
                ContentText = c.ContentText,
                NewsId = c.NewsId,
                UpVotes = 0,
                DownVotes = 0,
                Reports = 0,
                PostedById = c.PostedById,
                PostedDate = DateTime.Now,
                IsDeleted = false,
                UpdatedBy = null,
                LastUpdate = null

            };

            try
            {
                using (var entities = new CrsEntities())
                {
                    var news = entities.News.Single(t => t.Id == nComment.NewsId);
                    var count = entities.NewsComments.Where(t => !t.IsDeleted && t.NewsId == nComment.NewsId).Count();
                    news.Comments = count + 1;
                    // Add to DB
                    entities.NewsComments.Add(nComment);
                    entities.SaveChanges();

                    var user = entities.Users.Where(t => t.Id == nComment.PostedById).Select(t => new { t.Id, t.AvatarUrl, t.Username }).First();
                    nComment.PostedBy = new User { Id = user.Id, AvatarUrl = user.AvatarUrl, Username = user.Username };
                }
                return new Feedback<NewsComment>(true, null, nComment);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<NewsComment>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteNewsComment(NewsComment c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var cm = entities.NewsComments.Single(i => i.Id == c.Id);

                    if (cm.PostedById != c.UpdatedById)
                        return new Feedback(false, Messages.DeleteComment_NoPermission);

                    cm.IsDeleted = true;
                    cm.UpdatedById = cm.UpdatedById;
                    cm.LastUpdate = DateTime.Now;

                    var news = entities.News.Single(t => t.Id == cm.NewsId);
                    var count = entities.NewsComments.Where(t => !t.IsDeleted && t.NewsId == cm.NewsId).Count();
                    news.Comments = count - 1;

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

        public Feedback<NewsComment> VoteNewsComment(VotedNewsComment vc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var comment = entities.NewsComments.Single(i => i.Id == vc.CommentId);
                    var newVc = entities.VotedNewsComments.SingleOrDefault(t => t.CommentId == vc.CommentId && t.VotedById == vc.VotedById);

                    //Do not allow seft voting
                    if (comment.PostedById == vc.VotedById)
                    {
                        return new Feedback<NewsComment>(false, Messages.VoteComment_SeftVoting);
                    }

                    //Already voted with the same choice
                    if (newVc != null && newVc.IsUpVote == vc.IsUpVote)
                    {
                        return new Feedback<NewsComment>(true, null, new NewsComment { DownVotes = -1, UpVotes = -1 });
                    }

                    //Revote with other choice (Update)
                    if (newVc != null)
                    {
                        newVc.VotedDate = DateTime.Now;
                        newVc.IsUpVote = vc.IsUpVote;

                        if (vc.IsUpVote)
                            comment.DownVotes--;
                        else
                            comment.UpVotes--;
                    }
                    //Not vote yet (Insert)
                    else
                    {
                        newVc = new VotedNewsComment
                        {
                            Id = 0,
                            IsUpVote = vc.IsUpVote,
                            CommentId = vc.CommentId,
                            VotedById = vc.VotedById,
                            VotedDate = DateTime.Now,
                        };
                        entities.VotedNewsComments.Add(newVc);
                    }
                    if (vc.IsUpVote)
                        comment.UpVotes++;
                    else
                        comment.DownVotes++;


                    entities.SaveChanges();

                    return new Feedback<NewsComment>(true, null, comment);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<NewsComment>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportNewsComment(ReportedNewsComment rc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedNewsComments.Any(t => t.CommentId == rc.CommentId && !t.IsIgnored && t.ReportedById == rc.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRc = new ReportedNewsComment
                    {
                        IsIgnored = false,
                        CommentId = rc.CommentId,
                        Reason = rc.Reason,
                        ReportedById = rc.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedNewsComments.Add(newRc);

                    var comment = entities.NewsComments.Single(i => i.Id == rc.CommentId);
                    comment.Reports++;

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
    }
}