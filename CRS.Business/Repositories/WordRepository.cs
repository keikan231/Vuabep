using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class WordRepository : IWordRepository
    {
        public Feedback<IList<Word>> GetAllApprovedWords()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from w in entities.Words
                                 join u in entities.Users on w.CreatedById equals u.Id
                                 where !w.IsDeleted && !u.IsDeleted && w.IsApproved
                                 orderby w.Key
                                 select new
                                            {
                                                w.Id,
                                                w.Key,
                                                w.Value,
                                                w.CreatedById,
                                                u.Username,
                                                w.CreatedDate
                                            }).ToList();

                    IList<Word> words = new List<Word>();
                    if (query.Count > 0)
                    {
                        foreach (var item in query)
                        {
                            words.Add(new Word
                                          {
                                              Id = item.Id,
                                              Key = item.Key,
                                              Value = item.Value,
                                              CreatedById = item.CreatedById,
                                              CreatedBy = new User { Id = item.CreatedById, Username = item.Username },
                                              CreatedDate = item.CreatedDate
                                          });
                        }
                    }
                    return new Feedback<IList<Word>>(true, null, words);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Word>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<Word>> GetAllUnapprovedWords()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from w in entities.Words
                                 join u in entities.Users on w.CreatedById equals u.Id
                                 where !w.IsDeleted && !u.IsDeleted && !w.IsApproved
                                 select new
                                 {
                                     w.Id,
                                     w.Key,
                                     w.Value,
                                     w.CreatedById,
                                     u.Username,
                                     w.CreatedDate
                                 }).ToList();

                    IList<Word> words = new List<Word>();
                    if (query.Count > 0)
                    {
                        foreach (var item in query)
                        {
                            words.Add(new Word
                            {
                                Id = item.Id,
                                Key = item.Key,
                                Value = item.Value,
                                CreatedById = item.CreatedById,
                                CreatedBy = new User { Id = item.CreatedById, Username = item.Username },
                                CreatedDate = item.CreatedDate
                            });
                        }
                    }
                    return new Feedback<IList<Word>>(true, null, words);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Word>>(false, Messages.GeneralError);
            }
        }

        public Feedback<Word> ApproveWord(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var word = entities.Words.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    if (word == null)
                    {
                        return new Feedback<Word>(false, Messages.WordNotExisted);
                    }

                    word.IsApproved = true;
                    entities.SaveChanges();

                    return new Feedback<Word>(true, Messages.ApproveWordSuccess, word);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Word>(false, Messages.GeneralError);
            }
        }

        public Feedback<Word> UnapproveWord(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var word = entities.Words.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    if (word == null)
                    {
                        return new Feedback<Word>(false, Messages.WordNotExisted);
                    }

                    word.IsApproved = false;
                    entities.SaveChanges();

                    return new Feedback<Word>(true, Messages.UnapproveWordSuccess, word);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Word>(false, Messages.GeneralError);
            }
        }

        public Feedback<Word> InsertWord(Word w)
        {
            try
            {
                var word = new Word
                               {
                                   Key = w.Key,
                                   Value = w.Value,
                                   CreatedById = w.CreatedById,
                                   CreatedDate = DateTime.Now,
                                   IsApproved = false,
                                   ApprovedById = null,
                                   IsDeleted = false
                               };

                using (var entities = new CrsEntities())
                {                 
                    //Check for duplicate name
                    var exist = entities.Words
                        .FirstOrDefault(i => i.Key == word.Key && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<Word>(false, Messages.Dictionary_DublicateName);

                    // Add to DB
                    entities.Words.Add(word);
                    entities.SaveChanges();
                }

                return new Feedback<Word>(true, Messages.InsertWordSuccess, word);

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Word>(false, Messages.GeneralError);
            }
        }

        public Feedback<Word> GetWordForEditing(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var word = entities.Words.SingleOrDefault(t => t.Id == id && !t.IsDeleted && t.IsApproved);
                    return word != null ? new Feedback<Word>(true, null, word) : new Feedback<Word>(false, Messages.WordNotExisted);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Word>(false, Messages.GeneralError);
            }
        }

        public Feedback<Word> GetApprovedWordDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = (from q in entities.Words
                                 join u1 in entities.Users on q.CreatedById equals u1.Id
                                 where q.Id == id && !q.IsDeleted && q.IsApproved
                                 select new
                                 {
                                     WordId = q.Id,
                                     WordKey = q.Key,
                                     WordValue = q.Value,
                                     WordCreatedById = q.CreatedById,
                                     WordUsername = u1.Username,
                                 }).ToList();
                    if (query.Count <= 0) return new Feedback<Word>(false, Messages.WordNotExisted);

                    var word = new Word
                    {
                        Id = query[0].WordId,
                        Key = query[0].WordKey,
                        Value = query[0].WordKey,
                        CreatedBy = new User { Id = query[0].WordCreatedById, Username = query[0].WordUsername }
                    };

                    entities.SaveChanges();

                    return new Feedback<Word>(true, null, word);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Word>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteWord(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Word q = entities.Words.Single(i => i.Id == id);
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
    }
}