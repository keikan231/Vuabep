using System;
using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Web.Models;
using CRS.Web.Models.Searching;
using CRS.Web.ViewModels.Search;

namespace CRS.Web.Controllers
{
    public class SearchController : FrontEndControllerBase
    {
        /// <summary>
        /// Handles search pages and actions
        /// </summary>
        private INewsRepository _newsRepository;
        private ITipRepository _tipRepository;
        private IRecipeRepository _recipeRepository;
        private IAllContentRepository _allContentRepository;

        public SearchController(INewsRepository newsRepository, ITipRepository tipRepository, IAllContentRepository allContentRepository, IRecipeRepository recipeRepository)
        {
            _newsRepository = newsRepository;
            _tipRepository = tipRepository;
            _allContentRepository = allContentRepository;
            _recipeRepository = recipeRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuickSearchParser(SearchInput input)
        {
            input.NameUrl = input.Name;
            // Set to null to make it not appear in URL
            input.Name = null;
            return RedirectToActionPermanent("Result", "Search", input);
        }

        public ActionResult Result(SearchInput input)
        {
            var vm = new SearchResultViewModel { Input = input };
            // Criteria object to pass to repository
            var criteria = new SearchCriteria();

            input.Name = input.NameUrl;
            criteria.TitleSearch = input.NameUrl.ToSearchFriendly();

            // Get sort key
            criteria.OrderBy = GetSortKey(input.Sort);

            if (input.CategoryName == "news")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.NewsPageSize, input.Page ?? 1);

                // Search
                var feedback = _newsRepository.SearchNews(criteria);

                if (feedback.Success)
                {
                    vm.News = feedback.News;
                    vm.Total = feedback.TotalNews;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.News.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
                SetMessage(feedback.Message, MessageType.Error);
            }
            else if (input.CategoryName == "tips")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, input.Page ?? 1);

                // Search
                var feedback = _tipRepository.SearchTips(criteria);

                if (feedback.Success)
                {
                    vm.Tips = feedback.Tips;
                    vm.Total = feedback.TotalTips;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.Tips.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
                SetMessage(feedback.Message, MessageType.Error);
            }
            else if (input.CategoryName == "recipes")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, input.Page ?? 1);

                // Search
                var feedback = _recipeRepository.SearchRecipes(criteria);

                if (feedback.Success)
                {
                    vm.Recipes = feedback.Recipes;
                    vm.Total = feedback.TotalRecipes;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.Recipes.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
                SetMessage(feedback.Message, MessageType.Error);
            }
            else if (input.CategoryName == "all")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.AllPageSize, input.Page ?? 1);

                // Search
                var feedback = _allContentRepository.SearchAllContent(criteria);

                if (feedback.Success)
                {
                    vm.All = feedback.All;
                    vm.Total = feedback.Total;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.All.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ResultMore(SearchInput input)
        {
            if (input.Page == null)
                return Content("");

            var criteria = new SearchCriteria();
            criteria.TitleSearch = input.NameUrl.ToSearchFriendly();

            // Get sort key
            criteria.OrderBy = GetSortKey(input.Sort);

            if (input.CategoryName == "news")
            {

                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.NewsPageSize, input.Page ?? 1);

                // Search
                var feedback = _newsRepository.SearchNews(criteria);

                var vm = new SearchResultViewModel { Input = input };

                if (feedback.Success)
                {
                    vm.News = feedback.News;
                    vm.Total = feedback.TotalNews;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.News.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
            }
            else if (input.CategoryName == "tips")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, input.Page ?? 1);

                // Search
                var feedback = _tipRepository.SearchTips(criteria);

                var vm = new SearchResultViewModel { Input = input };

                if (feedback.Success)
                {
                    vm.Tips = feedback.Tips;
                    vm.Total = feedback.TotalTips;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.Tips.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
            }
            else if (input.CategoryName == "recipes")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, input.Page ?? 1);

                // Search
                var feedback = _recipeRepository.SearchRecipes(criteria);

                var vm = new SearchResultViewModel { Input = input };

                if (feedback.Success)
                {
                    vm.Recipes = feedback.Recipes;
                    vm.Total = feedback.TotalRecipes;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.Recipes.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
            }
            else if (input.CategoryName == "all")
            {
                // Get paging info
                criteria.PageInfo = new PageInfo(AppConfigs.AllPageSize, input.Page ?? 1);

                // Search
                var feedback = _allContentRepository.SearchAllContent(criteria);

                var vm = new SearchResultViewModel { Input = input };

                if (feedback.Success)
                {
                    vm.All = feedback.All;
                    vm.Total = feedback.Total;
                    vm.HasMore = vm.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo;
                    vm.NotFound = vm.All.Count == 0;
                    vm.OrderBy = criteria.OrderBy;

                    return View(vm);
                }
            }

            return Content("");
        }

        private Order GetSortKey(string sort)
        {
            // Default is sort by views
            if (sort == null)
            {
                return Order.Views;
            }

            if (sort.Equals("comments", StringComparison.OrdinalIgnoreCase))
                return Order.Comments;

            if (sort.Equals("rates", StringComparison.OrdinalIgnoreCase))
                return Order.Rates;

            if (sort.Equals("votes", StringComparison.OrdinalIgnoreCase))
                return Order.Votes;

            if (sort.Equals("approval", StringComparison.OrdinalIgnoreCase))
                return Order.Approval;

            return Order.Views;
        }


    }
}
