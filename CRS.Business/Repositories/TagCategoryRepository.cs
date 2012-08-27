using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Helpers;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class TagCategoryRepository : ITagCategoryRepository
    {
        #region Implementation of ITagCategoryRepository

        public Feedback<IList<TagCategory>> GetAllTagCategories()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    List<TagCategory> tagCategories =
                        entities.TagCategories
                        .Where(i => !i.IsDeleted)
                        .OrderBy(i => i.Name)
                        .ToList();
                    return new Feedback<IList<TagCategory>>(true, null, tagCategories);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<TagCategory>>(false, Messages.GeneralError);
            }
        }

        public Feedback<TagCategory> InsertTagCategory(TagCategory c)
        {
            TagCategory cnew = new TagCategory
            {
                Name = c.Name,
                NameUrl = c.NameUrl,
                Description = c.Description,
                Searches = 0,
                IsDeleted = false,
            };

            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    TagCategory exist = entities.TagCategories.FirstOrDefault(i => i.Name == cnew.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<TagCategory>(false, Messages.InsertTagCategory_DuplicateName);

                    // Add to DB
                    entities.TagCategories.Add(cnew);
                    entities.SaveChanges();

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(c.NameUrl))
                    {
                        cnew.NameUrl = cnew.Id.ToString();
                        entities.SaveChanges();
                    }
                    else
                    {
                        exist = entities.TagCategories.FirstOrDefault(
                                i => i.Id != cnew.Id && i.NameUrl == cnew.NameUrl && !i.IsDeleted);
                        if (exist != null)
                        {
                            cnew.NameUrl = string.Format("{0}-{1}", cnew.NameUrl, cnew.Id);
                            entities.SaveChanges();
                        }
                    }
                }

                return new Feedback<TagCategory>(true, null, cnew);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TagCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteTagCategory(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    TagCategory c = entities.TagCategories.Single(i => i.Id == id);
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

        public Feedback<TagCategory> GetTagCategoryDetails(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    TagCategory tagCategory = entities.TagCategories.SingleOrDefault(i => i.Id == id && !i.IsDeleted);
                    if (tagCategory != null)
                        return new Feedback<TagCategory>(true, null, tagCategory);
                    else
                        return new Feedback<TagCategory>(false, Messages.GetTagCategory_NotFound);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TagCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<TagCategory> UpdateTagCategory(TagCategory c)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    TagCategory exist = entities.TagCategories.FirstOrDefault(i => i.Id != c.Id && i.Name == c.Name && !i.IsDeleted);
                    if (exist != null)
                        return new Feedback<TagCategory>(false, Messages.InsertTagCategory_DuplicateName);

                    var tagCategory = entities.TagCategories.Single(i => i.Id == c.Id && !i.IsDeleted);
                    tagCategory.Name = c.Name;
                    tagCategory.Description = c.Description;

                    // Check for duplicate NameUrl
                    // TODO: using this format may still not eliminating duplication, but in general it would be fine
                    if (string.IsNullOrWhiteSpace(c.NameUrl))
                    {
                        tagCategory.NameUrl = c.Id.ToString();
                    }
                    else
                    {
                        exist = entities.TagCategories.FirstOrDefault(
                                i => i.Id != c.Id && i.NameUrl == c.NameUrl && !i.IsDeleted);
                        tagCategory.NameUrl = exist != null
                                               ? string.Format("{0}-{1}", c.NameUrl, c.Id)
                                               : c.NameUrl;
                    }

                    entities.SaveChanges();

                    return new Feedback<TagCategory>(true, null, tagCategory);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<TagCategory>(false, Messages.GeneralError);
            }
        }

        public Feedback<int> IncreaseSearchTimes(int id)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    int searches = entities.TagCategories.Where(i => i.Id == id).Select(i => i.Searches).Single();
                    TagCategory c = new TagCategory { Id = id }; // Assign Name to avoid EF's validation exception. This name won't be updated.
                    entities.TagCategories.Attach(c);
                    c.Searches = searches + 1;
                    entities.SaveChanges();

                    return new Feedback<int>(true, null, c.Searches);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<int>(false, Messages.GeneralError);
            }
        }

        public Feedback<TagCategory> InsertTagCategories(string tags)
        {
            if(tags == null)
                return new Feedback<TagCategory>(true);
            string[] tagsCategories = tags.Split(',');
            foreach (string tag in tagsCategories)
            {
                TagCategory cnew = new TagCategory
                {
                    Name = tag,
                    NameUrl = tag.ToUrlFriendly(),
                    Description = null,
                    Searches = 0,
                    IsDeleted = false,
                };
                Feedback feedback = InsertTagCategory(cnew);
                if(!feedback.Success)
                {
                    new Feedback<TagCategory>(false, Messages.GeneralError);
                }
            }

            return new Feedback<TagCategory>(true);
        }

        #endregion
    }
}