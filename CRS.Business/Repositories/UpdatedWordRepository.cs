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
    public class UpdatedWordRepository : IUpdatedWordRepository
    {
        public Feedback<IList<UpdatedWord>> GetAllUnapprovedUpdatedWords()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var query = from w in entities.UpdatedWords
                                join d in entities.Words on w.WordId equals d.Id
                                join u in entities.Users on w.UpdatedById equals u.Id
                                where !d.IsDeleted && !w.IsDeleted && d.IsApproved && !w.IsApproved && !u.IsDeleted
                                orderby w.Id
                                select new
                                {
                                    w.Id,
                                    w.WordId,
                                    d.Key,
                                    w.NewValue,
                                    w.UpdateDate,
                                    w.UpdatedById,
                                    u.Username
                                };
                    List<UpdatedWord> word = new List<UpdatedWord>();
                    foreach (var q in query)
                    {
                        word.Add(new UpdatedWord
                        {
                            Id = q.Id,
                            Words = new Word
                            {
                                Id = q.WordId,
                                Key = q.Key
                            },
                            NewValue = q.NewValue,
                            UpdateDate = q.UpdateDate,
                            UpdatedBy = new User
                            {
                                Id = q.UpdatedById,
                                Username = q.Username
                            },
                        });
                    }

                    return new Feedback<IList<UpdatedWord>>(true, null, word);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<UpdatedWord>>(false, Messages.GeneralError);
            }
        }

        public Feedback<UpdatedWord> ApproveUpdatedWord(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var word = entities.UpdatedWords.SingleOrDefault(t => t.Id == id && !t.IsDeleted);
                    word.IsApproved = true;

                    var word2 = entities.Words.SingleOrDefault(t => t.Id == word.WordId && !t.IsDeleted);
                    if(word2 == null)
                    {
                        return new Feedback<UpdatedWord>(false, Messages.WordNotExisted);
                    }
                    word2.Value = word.NewValue;
                    entities.SaveChanges();

                    return new Feedback<UpdatedWord>(true, Messages.ApproveUpdatedWordSuccess, word);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<UpdatedWord>(false, Messages.GeneralError);
            }
        }

        public Feedback<UpdatedWord> InsertUpdatedWord(UpdatedWord w)
        {
            try
            {
                var word = new UpdatedWord
                {
                    WordId = w.WordId,
                    NewValue = w.NewValue,
                    UpdatedById = w.UpdatedById,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false,
                    IsApproved = false,
                    ApprovedById = null
                };

                using (var entities = new CrsEntities())
                {
                    // Add to DB
                    entities.UpdatedWords.Add(word);
                    entities.SaveChanges();
                }

                return new Feedback<UpdatedWord>(true, Messages.UpdateWordSuccess, word);

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<UpdatedWord>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteUpdateWord(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    UpdatedWord q = entities.UpdatedWords.Single(i => i.Id == id);
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