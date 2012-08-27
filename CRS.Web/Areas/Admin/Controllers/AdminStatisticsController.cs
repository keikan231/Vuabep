using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Web.Areas.Admin.ViewModels.AdminStatistics;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    public class AdminStatisticsController : AdminControllerBase
    {
        private IApplicationRepository _repository;

        public AdminStatisticsController(IApplicationRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            StatisticsViewModel vm = new StatisticsViewModel();
            var feedback = _repository.GetStatistics();
            if (!feedback.Success)
            {
                SetMessage(feedback.Message, MessageType.Error);
                return View();
            }
            else
            {
                vm.AllUserNumber = feedback.AllUserNumber;
                vm.NewsNumber = feedback.NewsNumber;
                vm.TipNumber = feedback.TipNumber;
                vm.RecipeNumber = feedback.RecipeNumber;
                vm.ApprovedRecipeNumber = feedback.ApprovedRecipeNumber;
                vm.TipCategoryNumber = feedback.TipCategoryNumber;
                vm.RecipeCategoryNumber = feedback.RecipeCategoryNumber;
                vm.RecipeSmallCategoryNumber = feedback.RecipeSmallCategoryNumber;
                vm.NewsCommentNumber = feedback.NewsCommentNumber;
                vm.TipCategoryNumber = feedback.TipCommentNumber;
                vm.RecipeCommentNumber = feedback.RecipeCommentNumber;
                vm.QuestionNumber = feedback.QuestionNumber;
                vm.AnswerNumber = feedback.AnswerNumber;
                vm.VisitorNumber = feedback.VisitorNumber;
                vm.VisitorsToday = feedback.VisitorsToday;
                vm.OnlineVistorNumber = PageViewManager.GetOnlineVisitorNumber();


                return View(vm);
            }
        }

    }
}
