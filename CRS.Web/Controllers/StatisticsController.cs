using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Common.Logging;
using CRS.Web.Models.Tagging;

namespace CRS.Web.Controllers
{
    public class StatisticsController : FrontEndControllerBase
    {
        /// <summary>
        /// Handles statistics pages
        /// </summary>
        private readonly IQuestionRepository _questionRepository;
        private ITipRepository _tipRepository;
        private INewsRepository _newsRepository;
        private IUserRepository _userRepository;
        private IRecipeRepository _recipeRepository;

        public StatisticsController(IQuestionRepository questionRepository, ITipRepository tipRepository, INewsRepository newsRepository, IUserRepository userRepository, IRecipeRepository recipeRepository)
        {
            _questionRepository = questionRepository;
            _tipRepository = tipRepository;
            _newsRepository = newsRepository;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10' TODO: use cache profie once MVC resolves this bug
        public ActionResult TagCloud()
        {
            var categories = ReferenceDataCache.TagCategoryCollection
                .OrderByDescending(i => i.Searches)
                .Take(AppConfigs.TagCloudSize)
                .Select(i => new CategoryTag { Tag = i.Name, Weight = i.Searches, NameUrl = i.NameUrl });

            return View(categories);
        }

        public ActionResult TagCloudFull()
        {
            var categories = ReferenceDataCache.TagCategoryCollection
                .Select(i => new CategoryTag { Tag = i.Name, Weight = i.Searches, NameUrl = i.NameUrl });

            return View(categories);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult TopNews()
        {
            var feedback = _newsRepository.GetTopNews(AppConfigs.TopNewsNumber);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<News>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult TopTips()
        {
            var feedback = _tipRepository.GetTopTips(AppConfigs.TopTipNumber);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<Tip>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult TopRecipes()
        {
            var feedback = _recipeRepository.GetTopRecipes(AppConfigs.TopRecipeNumber);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<Recipe>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult TopRecipesByCategory(int id)
        {
            var feedback = _recipeRepository.GetTopRecipesByCategory(AppConfigs.TopRecipeNumber, id);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<Recipe>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult TopTipsByCategory(int id)
        {
            var feedback = _tipRepository.GetTopTipsByCategory(AppConfigs.TopTipByCategoryNumber, id);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<Tip>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 10)] // 10'
        public ActionResult TopMappedTipsByCategory(int recipeSmallCategoryId = 0)
        {
            int id;
            try
            {
                id = ReferenceDataCache.RecipeSmallCategoryCollection.FirstOrDefault(i => i.Id == recipeSmallCategoryId && !i.IsDeleted).Id;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                id = 0;
            }

            var feedback = _tipRepository.GetTopTipsByCategory(AppConfigs.TopMappedTipByCategoryNumber, id);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<Tip>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 540)] // 9'
        public ActionResult TopAnswerContributors()
        {
            var feedback = _questionRepository.GetTopAnswerContributors(AppConfigs.TopAnswerContributorNumber);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<User>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 540)] // 9'
        public ActionResult TopContributors()
        {
            var feedback = _userRepository.GetTopContributors(AppConfigs.TopContributorNumber);
            if (feedback.Success)
            {
                return View(feedback.Data);
            }

            return View(new List<User>());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 360)] // 6 mins
        public ActionResult NewestQuestions()
        {
            var feedback = _questionRepository.GetNewestQuestions(AppConfigs.NewQuestionNumber);

            return View(feedback.Data);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 360)] // 6 mins
        public ActionResult HotQuestions()
        {
            var feedback = _questionRepository.GetHotQuestionsInMonth(AppConfigs.NewQuestionNumber);

            return View(feedback.Data);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10 mins
        public ActionResult HeaderRecipeCategory()
        {
            var categories = ReferenceDataCache.RecipeCategoryCollection.ToList();
            return View(categories);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10 mins
        public ActionResult HeaderTipCategory()
        {
            var categories = ReferenceDataCache.TipCategoryCollection.ToList();
            return View(categories);
        }
    }
}
