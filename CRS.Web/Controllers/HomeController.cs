using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.DataSorting;
using CRS.Web.ViewModels.Sort;

namespace CRS.Web.Controllers
{
    public class HomeController : FrontEndControllerBase
    {
        /// <summary>
        /// Handles Index pages
        /// </summary>
        private ITipRepository _tipRepository;
        private INewsRepository _newsRepository;
        private IRecipeRepository _recipeRepository;

        public HomeController(ITipRepository tipRepository, INewsRepository newsRepository, IRecipeRepository recipeRepository)
        {
            _tipRepository = tipRepository;
            _newsRepository = newsRepository;
            _recipeRepository = recipeRepository;
        }

        public ActionResult Index(string sort, string content)
        {
            if (content != "news" && content != "tips" && content != "recipes")
            {
                content = "news";
            }
            var vm = new SortResultViewModel
                         {
                             NewsNotFound = true,
                             TipNotFound = true,
                             RecipeNotFound = true
                         }
                ;
            // Criteria object to pass to repository
            var criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.Content = content;
            var feedback1 = _newsRepository.SortNews(criteria, 5);
            var feedback2 = _tipRepository.SortTips(criteria, 5);
            var feedback3 = _recipeRepository.SortRecipes(criteria, 5);
            if (feedback1.Success && feedback2.Success)
            {
                vm.News = feedback1.News;
                vm.Tips = feedback2.Tips;
                vm.Recipes = feedback3.Recipes;
                vm.OrderBy = criteria.OrderBy;
                vm.Content = criteria.Content;
                vm.NewsNotFound = vm.News.Count == 0;
                vm.TipNotFound = vm.Tips.Count == 0;
                vm.RecipeNotFound = vm.Recipes.Count == 0;
            }
            return View(vm);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
