using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;
using CRS.Common.Helpers;

namespace CRS.Business.Repositories
{
    public class TipCategoryRepository : ITipCategoryRepository
    {
        public Feedback<IList<TipCategory>> GetAllTipCategories()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var categories =
                        entities.TipCategories.Where(t => !t.IsDeleted).OrderBy(t => t.Id).ToList();
                    return new Feedback<IList<TipCategory>>(true, null, categories);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<TipCategory>>(false, Messages.GeneralError);
            }
        }

        public Feedback<TipCategory> InsertTipCategory(TipCategory t)
        {
            TipCategory tnew = new TipCategory
                                   {
                                       Name = t.Name,
                                       NameUrl = t.NameUrl,
                                       Description = t.Description,
                                       IsDeleted = false
                                };

            try
            {
                using (var entities = new CrsEntities())
                {
                    //Check for duplicate name
                    var exist = entities.TipCategories
                        .FirstOrDefault(i => i.Name == tnew.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<TipCategory>(false, Messages.InsertCategory_DuplicateName);

                    entities.TipCategories.Add(tnew);
                    entities.SaveChanges();

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(t.NameUrl))
                    {
                        tnew.NameUrl = tnew.Id.ToString();
                        entities.SaveChanges();
                    }
                    else
                    {
                        exist = entities.TipCategories.FirstOrDefault(
                                i => i.Id != tnew.Id && i.NameUrl == tnew.NameUrl && !i.IsDeleted);
                        if (exist != null)
                        {
                            tnew.NameUrl = string.Format("{0}-{1}", tnew.NameUrl, tnew.Id);
                            entities.SaveChanges();
                        }
                    }
                }
                return new Feedback<TipCategory>(true, Messages.InsertTipCategorySuccess, tnew);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TipCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<TipCategory> GetTipCategoryDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    TipCategory category = entities.TipCategories.SingleOrDefault(i => i.Id == id && !i.IsDeleted);
                    if (category != null)
                        return new Feedback<TipCategory>(true, null, category);
                    else
                        return new Feedback<TipCategory>(false, Messages.GetCategory_NotFound);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TipCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<TipCategory> UpdateTipCategory(TipCategory c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    TipCategory exist = entities.TipCategories.FirstOrDefault(i => i.Id != c.Id && i.Name == c.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<TipCategory>(false, Messages.InsertCategory_DuplicateName);

                    var category = entities.TipCategories.Single(i => i.Id == c.Id && !i.IsDeleted);
                    category.Name = c.Name;
                    category.Description = c.Description;

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(c.NameUrl))
                    {
                        category.NameUrl = c.Id.ToString();
                    }
                    else
                    {
                        exist = entities.TipCategories.FirstOrDefault(
                            i => i.Id != c.Id && i.NameUrl == c.NameUrl && !i.IsDeleted);
                        category.NameUrl = exist != null
                                               ? string.Format("{0}-{1}", c.NameUrl, c.Id)
                                               : c.NameUrl;
                    }

                    entities.SaveChanges();

                    return new Feedback<TipCategory>(true, null, category);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TipCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteTipCategory(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    TipCategory c = entities.TipCategories.Single(i => i.Id == id);
                    c.IsDeleted = true;
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