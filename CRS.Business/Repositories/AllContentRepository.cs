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
    public class AllContentRepository : IAllContentRepository
    {
        #region Search All Content

        public SearchAllContentFeedback SearchAllContent(SearchCriteria criteria)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Prepare basic query
                    var query = (from t in entities.Tips
                                join u1 in entities.Users on t.PostedById equals u1.Id
                                 where t.TitleSearch.Contains(criteria.TitleSearch)
                                     && !t.IsDeleted
                                select new
                                {
                                    t.Id,
                                    t.Title,
                                    t.TitleUrl,
                                    t.ImageUrl,
                                    t.PostedDate,
                                    t.Views,
                                    t.Comments,
                                    t.ContentText,
                                    UserId = u1.Id,
                                    u1.Username,
                                    Content = "tips"
                                }).Concat(
                                from n in entities.News
                                join u2 in entities.Users on n.PostedById equals u2.Id
                                where n.TitleSearch.Contains(criteria.TitleSearch)
                                    && !n.IsDeleted
                                select new
                                {
                                    n.Id,
                                    n.Title,
                                    n.TitleUrl,
                                    n.ImageUrl,
                                    n.PostedDate,
                                    n.Views,
                                    n.Comments,
                                    n.ContentText,
                                    UserId = u2.Id,
                                    u2.Username,
                                    Content = "news"
                                }).Concat(
                                 from r in entities.Recipes
                                 join u3 in entities.Users on r.PostedById equals u3.Id
                                 where r.TitleSearch.Contains(criteria.TitleSearch)
                                     && !r.IsDeleted
                                 select new
                                 {
                                     r.Id,
                                     r.Title,
                                     r.TitleUrl,
                                     r.ImageUrl,
                                     r.PostedDate,
                                     r.Views,
                                     r.Comments,
                                     r.ContentText,
                                     UserId = u3.Id,
                                     u3.Username,
                                     Content = "recipes"
                                 });
                    SearchAllContentFeedback feedback = new SearchAllContentFeedback(true);

                    // Get total news count
                    feedback.Total = query.Count();

                    // Sorting
                    switch (criteria.OrderBy)
                    {
                        case Order.Views:
                            query = query.OrderByDescending(i => i.Views).ThenByDescending(i => i.Comments);
                            break;
                        case Order.Comments:
                            query = query.OrderByDescending(i => i.Comments).ThenByDescending(i => i.Views);
                            break;
                    }

                    // Paging
                    query = query
                        .Skip((criteria.PageInfo.PageNo - 1) * criteria.PageInfo.PageSize)
                        .Take(criteria.PageInfo.PageSize);

                    // Construct news list from queryable list

                    IList<Tuple<string, News>> all = new List<Tuple<string, News>>();
                    feedback.All = all;
                    foreach (var n in query)
                    {
                        all.Add(new Tuple<string, News>(n.Content, 
                            new News
                        {
                            Id = n.Id,
                            Title = n.Title,
                            TitleUrl = n.TitleUrl,
                            ImageUrl = n.ImageUrl,
                            PostedDate = n.PostedDate,
                            Views = n.Views,
                            Comments = n.Comments,
                            ContentText = n.ContentText,
                            PostedBy = new User
                            {
                                Id = n.UserId,
                                Username = n.Username
                            },
                        }));                 
                    }

                    return feedback;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new SearchAllContentFeedback(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}