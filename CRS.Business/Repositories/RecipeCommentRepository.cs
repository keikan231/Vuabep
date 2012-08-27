using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class RecipeCommentRepository : IRecipeCommentRepository
    {
        #region Manage reported Recipe Comments

        public Feedback<IList<RecipeComment>> GetAllReportedRecipeComments(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from c in entities.RecipeComments
                                    from u in entities.Users
                                    from p in entities.Recipes
                                    where c.PostedById == u.Id && c.RecipeId == p.Id && c.Reports >= minReportNumber && !c.IsDeleted
                                    orderby c.Reports descending
                                    select new
                                    {
                                        c.Id,
                                        c.ContentText,
                                        c.Reports,
                                        c.PostedDate,
                                        c.RecipeId,
                                        c.PostedById,
                                        p.Title,
                                        p.TitleUrl,
                                        u.Username
                                    };
                    List<RecipeComment> reportedRecipeComments = new List<RecipeComment>();
                    foreach (var q in queryable)
                    {
                        reportedRecipeComments.Add(new RecipeComment
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
                            Recipe = new Recipe
                            {
                                Id = q.RecipeId,
                                TitleUrl = q.TitleUrl,
                                Title = q.Title,
                            }
                        });
                    }

                    return new Feedback<IList<RecipeComment>>(true, null, reportedRecipeComments);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedComment(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get comment needed to be deleted
                    var c = entities.RecipeComments.Single(i => i.Id == id);
                    c.IsDeleted = true;

                    var recipe = entities.Recipes.Single(t => t.Id == c.RecipeId);
                    var count = entities.RecipeComments.Where(t => !t.IsDeleted && t.RecipeId == c.RecipeId).Count();
                    recipe.Comments = count - 1;

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
                    List<ReportedRecipeComment> rc = entities.ReportedRecipeComments.Where(i => i.CommentId == id).ToList();
                    foreach (ReportedRecipeComment i in rc)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    RecipeComment c = entities.RecipeComments.Single(i => i.Id == id);
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

        public Feedback<IList<ReportedRecipeComment>> GetReportedCommentDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedRecipeComments.Where(i => i.CommentId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedRecipeComment> reportedRecipeComments = new List<ReportedRecipeComment>();
                    foreach (var q in queryable)
                    {
                        reportedRecipeComments.Add(new ReportedRecipeComment
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
                    return new Feedback<IList<ReportedRecipeComment>>(true, null, reportedRecipeComments);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedRecipeComment>>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Recipe Comments

        public Feedback<IList<RecipeComment>> GetRecipeComments(int recipeId, PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from c in entities.RecipeComments
                                from u in entities.Users
                                where c.RecipeId == recipeId && c.PostedById == u.Id && !c.IsDeleted
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
                    IList<RecipeComment> recipeComments = new List<RecipeComment>();
                    foreach (var record in query)
                    {
                        recipeComments.Add(new RecipeComment
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

                    return new Feedback<IList<RecipeComment>>(true, null, recipeComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<RecipeComment>> GetHighlightRecipeComments(int recipeId, int number)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from c in entities.RecipeComments
                                 from u in entities.Users
                                 where c.RecipeId == recipeId && c.PostedById == u.Id && !c.IsDeleted && c.UpVotes - c.DownVotes > 0
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
                    IList<RecipeComment> recipeComments = new List<RecipeComment>();
                    foreach (var record in query)
                    {
                        recipeComments.Add(new RecipeComment
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

                    return new Feedback<IList<RecipeComment>>(true, null, recipeComments);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<RecipeComment>>(false, Messages.GeneralError);
            }
        }

        public Feedback<RecipeComment> InsertRecipeComment(RecipeComment c)
        {
            var nComment = new RecipeComment
            {
                ContentText = c.ContentText,
                RecipeId = c.RecipeId,
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
                    var recipe = entities.Recipes.Single(t => t.Id == nComment.RecipeId);
                    var count = entities.RecipeComments.Where(t => !t.IsDeleted && t.RecipeId == nComment.RecipeId).Count();
                    recipe.Comments = count + 1;
                    // Add to DB
                    entities.RecipeComments.Add(nComment);
                    entities.SaveChanges();

                    var user = entities.Users.Where(t => t.Id == nComment.PostedById).Select(t => new { t.Id, t.AvatarUrl, t.Username }).First();
                    nComment.PostedBy = new User { Id = user.Id, AvatarUrl = user.AvatarUrl, Username = user.Username };
                }
                return new Feedback<RecipeComment>(true, null, nComment);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeComment>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteRecipeComment(RecipeComment c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var cm = entities.RecipeComments.Single(i => i.Id == c.Id);

                    if (cm.PostedById != c.UpdatedById)
                        return new Feedback(false, Messages.DeleteComment_NoPermission);

                    cm.IsDeleted = true;
                    cm.UpdatedById = cm.UpdatedById;
                    cm.LastUpdate = DateTime.Now;

                    var recipe = entities.Recipes.Single(t => t.Id == cm.RecipeId);
                    var count = entities.RecipeComments.Where(t => !t.IsDeleted && t.RecipeId == cm.RecipeId).Count();
                    recipe.Comments = count - 1;

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

        public Feedback<RecipeComment> VoteRecipeComment(VotedRecipeComment vc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var comment = entities.RecipeComments.Single(i => i.Id == vc.CommentId);
                    var newVc = entities.VotedRecipeComments.SingleOrDefault(t => t.CommentId == vc.CommentId && t.VotedById == vc.VotedById);

                    //Do not allow seft voting
                    if (comment.PostedById == vc.VotedById)
                    {
                        return new Feedback<RecipeComment>(false, Messages.VoteComment_SeftVoting);
                    }

                    //Already voted with the same choice
                    if (newVc != null && newVc.IsUpVote == vc.IsUpVote)
                    {
                        return new Feedback<RecipeComment>(true, null, new RecipeComment { DownVotes = -1, UpVotes = -1 });
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
                        newVc = new VotedRecipeComment
                        {
                            Id = 0,
                            IsUpVote = vc.IsUpVote,
                            CommentId = vc.CommentId,
                            VotedById = vc.VotedById,
                            VotedDate = DateTime.Now,
                        };
                        entities.VotedRecipeComments.Add(newVc);
                    }
                    if (vc.IsUpVote)
                        comment.UpVotes++;
                    else
                        comment.DownVotes++;


                    entities.SaveChanges();

                    return new Feedback<RecipeComment>(true, null, comment);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<RecipeComment>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportRecipeComment(ReportedRecipeComment rc)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedRecipeComments.Any(t => t.CommentId == rc.CommentId && !t.IsIgnored && t.ReportedById == rc.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRc = new ReportedRecipeComment
                    {
                        IsIgnored = false,
                        CommentId = rc.CommentId,
                        Reason = rc.Reason,
                        ReportedById = rc.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedRecipeComments.Add(newRc);

                    var comment = entities.RecipeComments.Single(i => i.Id == rc.CommentId);
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