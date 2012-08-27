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
    public class TipCommentRepository : ITipCommentRepository
    {
        #region Manage reported Tip Comments

        public Feedback<IList<TipComment>> GetAllReportedTipComments(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from c in entities.TipComments
                                    from u in entities.Users
                                    from p in entities.Tips
                                    where c.PostedById == u.Id && c.TipId == p.Id && c.Reports >= minReportNumber && !c.IsDeleted
                                    orderby c.Reports descending
                                    select new
                                    {
                                        c.Id,
                                        c.ContentText,
                                        c.Reports,
                                        c.PostedDate,
                                        c.TipId,
                                        c.PostedById,
                                        p.Title,
                                        p.TitleUrl,
                                        u.Username
                                    };
                    List<TipComment> reportedTipComments = new List<TipComment>();
                    foreach (var q in queryable)
                    {
                        reportedTipComments.Add(new TipComment
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
                            Tip = new Tip
                            {
                                Id = q.TipId,
                                TitleUrl = q.TitleUrl,
                                Title = q.Title,
                            }
                        });
                    }

                    return new Feedback<IList<TipComment>>(true, null, reportedTipComments);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<TipComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedComment(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get comment needed to be deleted
                    var c = entities.TipComments.Single(i => i.Id == id);
                    c.IsDeleted = true;

                    var tip = entities.Tips.Single(t => t.Id == c.TipId);
                    var count = entities.TipComments.Where(t => !t.IsDeleted && t.TipId == c.TipId).Count();
                    tip.Comments = count - 1;

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
                    List<ReportedTipComment> rc = entities.ReportedTipComments.Where(i => i.CommentId == id).ToList();
                    foreach (ReportedTipComment i in rc)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    TipComment c = entities.TipComments.Single(i => i.Id == id);
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

        public Feedback<IList<ReportedTipComment>> GetReportedCommentDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedTipComments.Where(i => i.CommentId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedTipComment> reportedTipComments = new List<ReportedTipComment>();
                    foreach (var q in queryable)
                    {
                        reportedTipComments.Add(new ReportedTipComment
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
                    return new Feedback<IList<ReportedTipComment>>(true, null, reportedTipComments);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedTipComment>>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Tip Comments

        public Feedback<IList<TipComment>> GetTipComments(int tipId, PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from c in entities.TipComments
                                from u in entities.Users
                                where c.TipId == tipId && c.PostedById == u.Id && !c.IsDeleted
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
                    IList<TipComment> tipComments = new List<TipComment>();
                    foreach (var record in query)
                    {
                        tipComments.Add(new TipComment
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

                    return new Feedback<IList<TipComment>>(true, null, tipComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<TipComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<TipComment>> GetHighlightTipComments(int tipId, int number)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from c in entities.TipComments
                                 from u in entities.Users
                                 where c.TipId == tipId && c.PostedById == u.Id && !c.IsDeleted && c.UpVotes - c.DownVotes > 0
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
                    IList<TipComment> tipComments = new List<TipComment>();
                    foreach (var record in query)
                    {
                        tipComments.Add(new TipComment
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

                    return new Feedback<IList<TipComment>>(true, null, tipComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<TipComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<TipComment> InsertTipComment(TipComment c)
        {
            var nComment = new TipComment
            {
                ContentText = c.ContentText,
                TipId = c.TipId,
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
                    var tip = entities.Tips.Single(t => t.Id == nComment.TipId);
                    var count = entities.TipComments.Where(t => !t.IsDeleted && t.TipId == nComment.TipId).Count();
                    tip.Comments = count + 1;
                    // Add to DB
                    entities.TipComments.Add(nComment);
                    entities.SaveChanges();

                    var user = entities.Users.Where(t => t.Id == nComment.PostedById).Select(t => new { t.Id, t.AvatarUrl, t.Username }).First();
                    nComment.PostedBy = new User { Id = user.Id, AvatarUrl = user.AvatarUrl, Username = user.Username };
                }
                return new Feedback<TipComment>(true, null, nComment);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TipComment>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteTipComment(TipComment c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var cm = entities.TipComments.Single(i => i.Id == c.Id);

                    if (cm.PostedById != c.UpdatedById)
                        return new Feedback(false, Messages.DeleteComment_NoPermission);

                    cm.IsDeleted = true;
                    cm.UpdatedById = cm.UpdatedById;
                    cm.LastUpdate = DateTime.Now;

                    var tip = entities.Tips.Single(t => t.Id == cm.TipId);
                    var count = entities.TipComments.Where(t => !t.IsDeleted && t.TipId == cm.TipId).Count();
                    tip.Comments = count - 1;

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

        public Feedback<TipComment> VoteTipComment(VotedTipComment vc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var comment = entities.TipComments.Single(i => i.Id == vc.CommentId);
                    var newVc = entities.VotedTipComments.SingleOrDefault(t => t.CommentId == vc.CommentId && t.VotedById == vc.VotedById);

                    //Do not allow seft voting
                    if (comment.PostedById == vc.VotedById)
                    {
                        return new Feedback<TipComment>(false, Messages.VoteComment_SeftVoting);
                    }

                    //Already voted with the same choice
                    if (newVc != null && newVc.IsUpVote == vc.IsUpVote)
                    {
                        return new Feedback<TipComment>(true, null, new TipComment { DownVotes = -1, UpVotes = -1 });
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
                        newVc = new VotedTipComment
                        {
                            Id = 0,
                            IsUpVote = vc.IsUpVote,
                            CommentId = vc.CommentId,
                            VotedById = vc.VotedById,
                            VotedDate = DateTime.Now,
                        };
                        entities.VotedTipComments.Add(newVc);
                    }
                    if (vc.IsUpVote)
                        comment.UpVotes++;
                    else
                        comment.DownVotes++;


                    entities.SaveChanges();

                    return new Feedback<TipComment>(true, null, comment);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TipComment>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportTipComment(ReportedTipComment rc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedTipComments.Any(t => t.CommentId == rc.CommentId && !t.IsIgnored && t.ReportedById == rc.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRc = new ReportedTipComment
                    {
                        IsIgnored = false,
                        CommentId = rc.CommentId,
                        Reason = rc.Reason,
                        ReportedById = rc.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedTipComments.Add(newRc);

                    var comment = entities.TipComments.Single(i => i.Id == rc.CommentId);
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