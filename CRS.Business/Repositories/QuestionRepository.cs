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
    public class QuestionRepository : IQuestionRepository
    {
        #region Reported Question

        public Feedback<IList<Question>> GetAllReportedQuestion(int minReportNumber)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = from q in entities.Questions
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
                    List<Question> rq = new List<Question>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new Question
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

                    return new Feedback<IList<Question>>(true, null, rq);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Question>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<ReportedQuestion>> GetReportedQuestionDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var queryable = entities.ReportedQuestions.Where(i => i.QuestionId == id)
                        .Select(i => new
                        {
                            i.Id,
                            i.ReportedDate,
                            i.Reason,
                            i.ReportedBy.Username
                        });

                    List<ReportedQuestion> rq = new List<ReportedQuestion>();
                    foreach (var q in queryable)
                    {
                        rq.Add(new ReportedQuestion
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
                    return new Feedback<IList<ReportedQuestion>>(true, null, rq);

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<ReportedQuestion>>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteReportedQuestion(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Get question needed to be deleted
                    Question q = entities.Questions.Single(i => i.Id == id);
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
                    List<ReportedQuestion> rq = entities.ReportedQuestions.Where(i => i.QuestionId == id).ToList();
                    foreach (ReportedQuestion i in rq)
                    {
                        i.IsIgnored = true;
                    }
                    //set report number to zero
                    Question q = entities.Questions.Single(i => i.Id == id);
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

        #region Q&A

        public ListQuestionFeedback GetAllQuestions(PageInfo pageInfo)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from q in entities.Questions
                                join u in entities.Users on q.PostedById equals u.Id
                                where !q.IsDeleted
                                orderby q.PostedDate descending
                                select new
                                {
                                    q.Id,
                                    q.Title,
                                    q.TitleUrl,
                                    q.PostedDate,
                                    q.Views,
                                    q.ContentText,
                                    q.AnswerTimes,
                                    UserId = u.Id,
                                    u.Username
                                };
                    var total = query.Count();
                    query = query.Skip((pageInfo.PageNo - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
                    var questions = new List<Question>();
                    foreach (var q in query)
                    {
                        questions.Add(new Question
                        {
                            Id = q.Id,
                            Title = q.Title,
                            TitleUrl = q.TitleUrl,
                            PostedBy = new User { Id = q.UserId, Username = q.Username },
                            PostedDate = q.PostedDate,
                            Views = q.Views,
                            ContentText = q.ContentText,
                            AnswerTimes = q.AnswerTimes
                        });
                    }

                    return new ListQuestionFeedback(true, null) { Questions = questions, Total = total };
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ListQuestionFeedback(false, Messages.GeneralError);
            }
        }

        public Feedback<Question> InsertQuestion(Question q)
        {
            var qnew = new Question
            {
                Title = q.Title,
                TitleUrl = q.TitleUrl,
                ContentText = q.ContentText,
                Views = 0,
                AnswerTimes = 0,
                Reports = 0,
                PostedById = q.PostedById,
                PostedDate = DateTime.Now,
                IsDeleted = false,
                UpdatedBy = null,
                LastUpdate = null
            };

            try
            {
                using (var entities = new CrsEntities())
                {
                    // Add to DB
                    entities.Questions.Add(qnew);
                    entities.SaveChanges();
                }

                return new Feedback<Question>(true, null, qnew);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Question>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteQuestion(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Question q = entities.Questions.Single(i => i.Id == id);
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

        public Feedback<Question> GetQuestionForEditing(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var question = entities.Questions.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    return question != null ? new Feedback<Question>(true, null, question) : new Feedback<Question>(false, Messages.QuestionNotExisted);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Question>(false, Messages.GeneralError);
            }
        }

        public Feedback<Question> GetQuestionDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query1 = (from q in entities.Questions
                                  join u1 in entities.Users on q.PostedById equals u1.Id
                                  where q.Id == id && !q.IsDeleted
                                  select new
                                  {
                                      QuestionId = q.Id,
                                      q.Title,
                                      q.TitleUrl,
                                      q.Views,
                                      QuestionPostedDate = q.PostedDate,
                                      QuestionPostedById = q.PostedById,
                                      QuestioContentText = q.ContentText,
                                      QuestionUsername = u1.Username,
                                      QuestionAvatarUrl = u1.AvatarUrl,
                                      QuestionUserLevel = u1.Level,
                                  }).ToList();
                    if (query1.Count <= 0) return new Feedback<Question>(false, Messages.QuestionNotExisted);

                    var question = new Question
                    {
                        Id = query1[0].QuestionId,
                        Title = query1[0].Title,
                        TitleUrl = query1[0].TitleUrl,
                        PostedById = query1[0].QuestionPostedById,
                        PostedDate = query1[0].QuestionPostedDate,
                        PostedBy = new User { Id = query1[0].QuestionPostedById, Username = query1[0].QuestionUsername, AvatarUrl = query1[0].QuestionAvatarUrl , Level = query1[0].QuestionUserLevel },
                        ContentText = query1[0].QuestioContentText,
                        Answers = new List<Answer>(),
                        Views = query1[0].Views
                    };

                    var tmpQues = entities.Questions.Single(i => i.Id == id);
                    tmpQues.Views++;
                    entities.SaveChanges();

                    return new Feedback<Question>(true, null, question);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Question>(false, Messages.GeneralError);
            }
        }

        public Feedback<Question> UpdateQuestion(Question q)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var question = entities.Questions.SingleOrDefault(i => i.Id == q.Id && !q.IsDeleted);

                    if (question == null)
                        return new Feedback<Question>(false, Messages.QuestionNotExisted);

                    question.Title = q.Title;
                    question.ContentText = q.ContentText;
                    question.UpdatedById = q.UpdatedById;
                    question.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback<Question>(true, null, question);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Question>(false, Messages.GeneralError);
            }
        }

        public Feedback ReportQuestion(ReportedQuestion rq)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    if (entities.ReportedQuestions.Any(t => t.QuestionId == rq.QuestionId && !t.IsIgnored && t.ReportedById == rq.ReportedById))
                    {
                        return new Feedback(true, string.Empty);
                    }

                    var newRq = new ReportedQuestion
                    {
                        IsIgnored = false,
                        QuestionId = rq.QuestionId,
                        Reason = rq.Reason,
                        ReportedById = rq.ReportedById,
                        ReportedDate = DateTime.Now,
                    };


                    entities.ReportedQuestions.Add(newRq);

                    var q = entities.Questions.Single(i => i.Id == rq.QuestionId);
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

        #region Hot questions, answers

        public Feedback<IList<Question>> GetHotQuestionsInMonth(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var fromDate = DateTime.Now.AddMonths(-1);

                    var query = entities.Questions
                        .Where(i => i.PostedDate >= fromDate && !i.IsDeleted)
                        .OrderByDescending(i => i.Views)
                        .Select(i => new { i.Id, i.Title, i.TitleUrl, i.Views })
                        .Take(take);
                    var questions = new List<Question>();
                    foreach (var item in query)
                    {
                        questions.Add(new Question { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, Views = item.Views });
                    }

                    return new Feedback<IList<Question>>(true, null, questions);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Question>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<User>> GetTopAnswerContributors(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = entities.Users
                        .Select(i => new { i.Id, i.Username, NumberOfAnswers = i.Answers.Count(a => !a.IsDeleted), i.AvatarUrl })
                        .Where(i => i.NumberOfAnswers > 0)
                        .OrderByDescending(i => i.NumberOfAnswers)
                        .Take(take);

                    var users = new List<User>();
                    foreach (var item in query)
                    {
                        users.Add(new User { Id = item.Id, Username = item.Username, NumberOfAnswers = item.NumberOfAnswers, AvatarUrl = item.AvatarUrl });
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

        public Feedback<IList<Question>> GetNewestQuestions(int take)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = entities.Questions
                        .Where(i => !i.IsDeleted)
                        .OrderByDescending(i => i.PostedDate)
                        .Select(i => new { i.Id, i.Title, i.TitleUrl, i.Views })
                        .Take(take);

                    var questions = new List<Question>();
                    foreach (var item in query)
                    {
                        questions.Add(new Question { Id = item.Id, Title = item.Title, TitleUrl = item.TitleUrl, Views = item.Views });
                    }

                    return new Feedback<IList<Question>>(true, null, questions);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Question>>(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}