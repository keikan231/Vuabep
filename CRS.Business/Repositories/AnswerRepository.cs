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
    public class AnswerRepository : IAnswerRepository
    {
        public Feedback<IList<Answer>> GetAllReportedAnswers(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from a in entities.Answers
                                    from u in entities.Users
                                    from q in entities.Questions
                                    where !a.IsDeleted && !q.IsDeleted && a.Reports >= minReportNumber && a.PostedById == u.Id && a.QuestionId == q.Id
                                    orderby a.Reports descending
                                    select new
                                    {
                                        a.Id,
                                        a.Reports,
                                        a.PostedDate,
                                        a.PostedById,
                                        a.QuestionId,
                                        a.ContentText,
                                        u.Username,
                                        q.Title,
                                        q.TitleUrl
                                    };
                    List<Answer> rq = new List<Answer>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new Answer
                        {
                            Id = q.Id,
                            PostedDate = q.PostedDate,
                            Reports = q.Reports,
                            PostedBy = new User
                            {
                                Id = q.PostedById,
                                Username = q.Username
                            },
                            Question = new Question
                            {
                                Id = q.QuestionId,
                                Title = q.Title,
                                TitleUrl = q.TitleUrl
                            },
                            ContentText = q.ContentText
                        });
                    }

                    return new Feedback<IList<Answer>>(true, null, rq);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Answer>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<ReportedAnswer>> GetReportedAnswerDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedAnswers.Where(i => i.AnswerId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedAnswer> ra = new List<ReportedAnswer>();
                    foreach (var q in queryable)
                    {
                        ra.Add(new ReportedAnswer
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
                    return new Feedback<IList<ReportedAnswer>>(true, null, ra);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedAnswer>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedAnswer(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get answer needed to be deleted
                    var a = entities.Answers.Single(i => i.Id == id);

                    var question = entities.Questions.Single(t => t.Id == a.QuestionId);
                    var count = entities.Answers.Where(t => !t.IsDeleted && t.QuestionId == a.QuestionId).Count();
                    question.AnswerTimes = count - 1;
                    a.IsDeleted = true;

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
                    // Delete attached reports of the answer
                    List<ReportedAnswer> ra = entities.ReportedAnswers.Where(i => i.AnswerId == id).ToList();
                    foreach (ReportedAnswer i in ra)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    Answer a = entities.Answers.Single(i => i.Id == id);
                    a.Reports = 0;
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

        public ListAnswerFeedback GetAllAnswers(int id, PageInfo pageInfo)
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
                                 where !a.IsDeleted && !q.IsDeleted && a.QuestionId == id
                                 select new
                                 {
                                     AnswerId = a.Id,
                                     AnswerContentText = a.ContentText,
                                     AnswerPostedById = a.PostedById,
                                     AnswerPostedDate = a.PostedDate,
                                     AnswerDownVotes = a.DownVotes,
                                     AnswerUpVotes = a.UpVotes,
                                     AnswerUsername = u2.Username,
                                     AnswerAvatarUrl = u2.AvatarUrl,
                                 });

                    var question = entities.Questions.FirstOrDefault(t => t.Id == id);
                    if (question == null)
                        return new ListAnswerFeedback(false, Messages.QuestionNotExisted);
                    var total = question.AnswerTimes;

                    query = query.Skip((pageInfo.PageNo - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
                    foreach (var q in query)
                    {
                        answers.Add(new Answer
                        {
                            Id = q.AnswerId,
                            ContentText = q.AnswerContentText,
                            PostedById = q.AnswerPostedById,
                            PostedDate = q.AnswerPostedDate,
                            PostedBy = new User { Id = q.AnswerPostedById, Username = q.AnswerUsername, AvatarUrl = q.AnswerAvatarUrl },
                            QuestionId = id,
                            UpVotes = q.AnswerUpVotes,
                            DownVotes = q.AnswerDownVotes,
                        });
                    }

                    return new ListAnswerFeedback(true, null) { Answers = answers, Total = total };
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListAnswerFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback<Answer> InsertAnswer(Answer a)
        {
            var answer = new Answer
            {
                QuestionId = a.QuestionId,
                ContentText = a.ContentText,
                PostedById = a.PostedById,
                PostedDate = DateTime.Now,
                LastUpdate = null,
                UpdatedBy = null,
                IsDeleted = false,
            };
            try
            {
                using (var entities = new CrsEntities())
                {
                    var question = entities.Questions.Single(t => t.Id == answer.QuestionId);
                    var count = entities.Answers.Where(t => !t.IsDeleted && t.QuestionId == answer.QuestionId).Count();
                    question.AnswerTimes = count + 1;
                    // Add to DB
                    entities.Answers.Add(answer);
                    entities.SaveChanges();
                    var user = entities.Users.Where(t => t.Id == answer.PostedById).Select(t => new { t.Id, t.AvatarUrl, t.Username }).First();
                    answer.PostedBy = new User { Id = user.Id, AvatarUrl = user.AvatarUrl, Username = user.Username };
                }
                return new Feedback<Answer>(true, null, answer);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Answer>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteAnswer(Answer answer)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var a = entities.Answers.Single(i => i.Id == answer.Id);

                    if (a.PostedById != answer.UpdatedById)
                        return new Feedback(false, Messages.DeleteAnswer_NoPermission);

                    var question = entities.Questions.Single(t => t.Id == a.QuestionId);
                    var count = entities.Answers.Where(t => !t.IsDeleted && t.QuestionId == a.QuestionId).Count();
                    question.AnswerTimes = count - 1;

                    a.IsDeleted = true;
                    a.UpdatedById = a.UpdatedById;
                    a.LastUpdate = DateTime.Now;

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

        public Feedback<Answer> VoteAnswer(VotedAnswer va)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var answer = entities.Answers.Single(i => i.Id == va.AnswerId);
                    var newVa = entities.VotedAnswers.SingleOrDefault(t => t.AnswerId == va.AnswerId && t.VotedById == va.VotedById);

                    //Do not allow seft voting
                    if (answer.PostedById == va.VotedById)
                    {
                        return new Feedback<Answer>(false, Messages.VoteAnswer_SeftVoting);
                    }

                    //Already voted with the same choice
                    if (newVa != null && newVa.IsUpVote == va.IsUpVote)
                    {
                        return new Feedback<Answer>(true, null, new Answer { DownVotes = -1, UpVotes = -1 });
                    }

                    //Revote with other choice (Update)
                    if (newVa != null)
                    {
                        newVa.VotedDate = DateTime.Now;
                        newVa.IsUpVote = va.IsUpVote;

                        if (va.IsUpVote)
                            answer.DownVotes--;
                        else
                            answer.UpVotes--;
                    }
                    //Not vote yet (Insert)
                    else
                    {
                        newVa = new VotedAnswer
                        {
                            Id = 0,
                            IsUpVote = va.IsUpVote,
                            AnswerId = va.AnswerId,
                            VotedById = va.VotedById,
                            VotedDate = DateTime.Now,
                        };
                        entities.VotedAnswers.Add(newVa);
                    }
                    if (va.IsUpVote)
                        answer.UpVotes++;
                    else
                        answer.DownVotes++;


                    entities.SaveChanges();

                    return new Feedback<Answer>(true, null, answer);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Answer>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportAnswer(ReportedAnswer ra)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedAnswers.Any(t => t.AnswerId == ra.AnswerId && !t.IsIgnored && t.ReportedById == ra.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRa = new ReportedAnswer
                    {
                        IsIgnored = false,
                        AnswerId = ra.AnswerId,
                        Reason = ra.Reason,
                        ReportedById = ra.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedAnswers.Add(newRa);

                    var answer = entities.Answers.Single(i => i.Id == ra.AnswerId);
                    answer.Reports++;

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

        public Feedback<IList<Answer>> GetHighlightAnswers(int id, int number)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var answers = new List<Answer>();
                    var query = (from a in entities.Answers
                                 join u2 in entities.Users on a.PostedById equals u2.Id
                                 orderby a.UpVotes - a.DownVotes descending
                                 where !a.IsDeleted && a.UpVotes - a.DownVotes > 0 && a.QuestionId == id
                                 select new
                                 {
                                     AnswerId = a.Id,
                                     AnswerPostedById = a.PostedById,
                                     AnswerPostedDate = a.PostedDate,
                                     AnswerDownVotes = a.DownVotes,
                                     AnswerUpVotes = a.UpVotes,
                                     AnswerUsername = u2.Username,
                                     AnswerAvatarUrl = u2.AvatarUrl,
                                     AnswerContentText = a.ContentText
                                 }).Take(number);
                    foreach (var q in query)
                    {
                        answers.Add(new Answer
                        {
                            Id = q.AnswerId,
                            PostedById = q.AnswerPostedById,
                            PostedDate = q.AnswerPostedDate,
                            PostedBy = new User { Id = q.AnswerPostedById, Username = q.AnswerUsername, AvatarUrl = q.AnswerAvatarUrl },
                            QuestionId = id,
                            UpVotes = q.AnswerUpVotes,
                            DownVotes = q.AnswerDownVotes,
                            ContentText = q.AnswerContentText
                        });
                    }

                    return new Feedback<IList<Answer>>(true, null, answers);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Answer>>(false, Messages.GeneralError);
            }
        }
    }
}