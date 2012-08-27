using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Common;
using CRS.Resources;
using CRS.Web.Areas.Admin.ViewModels.ManageContents;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;
using Telerik.Web.Mvc;

namespace CRS.Web.Areas.Admin.Controllers
{
    public class ManageContentsController : AdminControllerBase
    {
        private INewsRepository _newsRepository;
        private ITipRepository _tipRepository;
        private IRecipeRepository _recipeRepository;
        private IQuestionRepository _questionRepository;

         #region Constructors

        public ManageContentsController(INewsRepository newsRepository, ITipRepository tipRepository, IQuestionRepository questionRepository, IRecipeRepository recipeRepository)
        {
            _newsRepository = newsRepository;
            _tipRepository = tipRepository;
            _recipeRepository = recipeRepository;
            _questionRepository = questionRepository;
        }

        #endregion

        #region Manage News

        [GridAction(GridName = "Grid")]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult ManageNews(GridCommand gridCommand)
        {
            ListNewsViewModel vm = new ListNewsViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["News-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));

            SortCriteria criteria = new SortCriteria();
            criteria.PageInfo = pageInfo;

            var feedback = _newsRepository.GetAllNews(criteria);

            //Set default value for Total record
            ViewData["Total"] = 0;

            if (feedback.Success)
            {
                vm.News = feedback.News;
                ViewData["Total"] = feedback.Total;
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult DeleteNews(int id)
        {
            var feedback = _newsRepository.DeleteNews(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteNewsSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ManageNews");
        }

        #endregion

        #region Manage Tips

        [GridAction(GridName = "Grid")]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult ManageTips(GridCommand gridCommand)
        {
            ListTipsViewModel vm = new ListTipsViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["Tips-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));

            SortCriteria criteria = new SortCriteria();
            criteria.PageInfo = pageInfo;

            var feedback = _tipRepository.GetAllTip(criteria);

            //Set default value for Total record
            ViewData["Total"] = 0;

            if (feedback.Success)
            {
                vm.Tips = feedback.Tips;
                ViewData["Total"] = feedback.Total;
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult DeleteTips(int id)
        {
            var feedback = _tipRepository.DeleteTip(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteTipSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ManageTips");
        }

        #endregion

        #region Manage Questions

        [GridAction(GridName = "Grid")]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult ManageQuestions(GridCommand gridCommand)
        {
            ListQuestionsViewModel vm = new ListQuestionsViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["Questions-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));

            var feedback = _questionRepository.GetAllQuestions(pageInfo);

            //Set default value for Total record
            ViewData["Total"] = 0;

            if (feedback.Success)
            {
                vm.Questions = feedback.Questions;
                ViewData["Total"] = feedback.Total;
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult DeleteQuestion(int id)
        {
            var feedback = _questionRepository.DeleteQuestion(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteTipSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ManageQuestions");
        }

        #endregion

        #region Manage Recipes

        [GridAction(GridName = "Grid")]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult ManageRecipes(GridCommand gridCommand)
        {
            ListRecipesViewModel vm = new ListRecipesViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["Recipes-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));

            SortCriteria criteria = new SortCriteria();
            criteria.PageInfo = pageInfo;

            var feedback = _recipeRepository.GetAllRecipe(criteria);

            //Set default value for Total record
            ViewData["Total"] = 0;

            if (feedback.Success)
            {
                vm.Recipes = feedback.Recipes;
                ViewData["Total"] = feedback.Total;
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(Permissions = KeyObject.Permission.ManageContents)]
        public ActionResult DeleteRecipes(int id)
        {
            var feedback = _recipeRepository.DeleteRecipe(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteRecipeSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ManageRecipes");
        }

        #endregion
    }
}
