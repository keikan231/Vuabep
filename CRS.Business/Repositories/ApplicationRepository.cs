using System;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        #region Implementation of IApplicationRepository

        public Feedback<int> IncreasePageView()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var exist = entities.VisitorNumbers.FirstOrDefault(i => i.Date == DateTime.Today);
                    if (exist != null)
                    {
                        exist.Visitors++;
                    }
                    else
                    {
                        exist = new VisitorNumber { Visitors = 1, Date = DateTime.Today };
                        entities.VisitorNumbers.Add(exist);
                    }

                    entities.SaveChanges();

                    return new Feedback<int>(true, null, exist.Visitors);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<int>(false, Messages.GeneralError);
            }
        }

        public StatisticsFeedback GetStatistics()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    StatisticsFeedback feedback = new StatisticsFeedback(true, null);
     
                    feedback.TipNumber = (from t in entities.Tips
                                          join c in entities.TipCategories on t.TipCategoryId equals c.Id
                                          where !t.IsDeleted && !c.IsDeleted
                                          select new { t.Id }).Count();

                    feedback.RecipeNumber = (from r in entities.Recipes
                                             join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                             join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                             join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                             where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted
                                             select new { r.Id }).Count();

                    feedback.ApprovedRecipeNumber = (from r in entities.Recipes
                                                     join m in entities.RecipeCategoryMappings on r.MappingCategoryId equals m.Id
                                                     join c in entities.RecipeCategories on m.RecipeCategoryId equals c.Id
                                                     join s in entities.RecipeSmallCategories on m.RecipeSmallCategoryId equals s.Id
                                                     where !r.IsDeleted && !c.IsDeleted && !s.IsDeleted && r.IsApproved
                                                     select new { r.Id }).Count();

                    feedback.NewsCommentNumber = (from c in entities.NewsComments
                                                  join n in entities.News on c.NewsId equals n.Id
                                                  where !c.IsDeleted && !n.IsDeleted
                                                  select new { c.Id }).Count();

                    feedback.TipCommentNumber = (from c in entities.TipComments
                                                 join n in entities.Tips on c.TipId equals n.Id
                                                 where !c.IsDeleted && !n.IsDeleted
                                                 select new { c.Id }).Count();

                    feedback.RecipeCommentNumber = (from c in entities.RecipeComments
                                                    join n in entities.Recipes on c.RecipeId equals n.Id
                                                    where !c.IsDeleted && !n.IsDeleted
                                                    select new { c.Id }).Count();

                    feedback.AnswerNumber = (from c in entities.Answers
                                             join n in entities.Questions on c.QuestionId equals n.Id
                                             where !c.IsDeleted && !n.IsDeleted
                                             select new { c.Id }).Count();

                    feedback.AllUserNumber = entities.Users.Count(i => !i.IsDeleted);
                    feedback.NewsNumber = entities.News.Count(i => !i.IsDeleted);
                    feedback.TipCategoryNumber = entities.TipCategories.Count(i => !i.IsDeleted);
                    feedback.RecipeCategoryNumber = entities.RecipeCategories.Count(i => !i.IsDeleted);
                    feedback.RecipeSmallCategoryNumber = entities.RecipeSmallCategories.Count(i => !i.IsDeleted);
                    feedback.QuestionNumber = entities.Questions.Count(i => !i.IsDeleted);
                    feedback.VisitorNumber = entities.VisitorNumbers.Sum(i => (int?)i.Visitors) ?? 0;
                    var today = entities.VisitorNumbers.SingleOrDefault(i => i.Date == DateTime.Today);
                    feedback.VisitorsToday = today != null ? today.Visitors : 0;

                    return feedback;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new StatisticsFeedback(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}