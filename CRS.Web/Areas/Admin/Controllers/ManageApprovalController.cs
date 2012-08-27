using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Common;
using CRS.Resources;
using CRS.Web.Areas.Admin.ViewModels.ManageApproval;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles categories management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageApproval)]
    public class ManageApprovalController : AdminControllerBase
    {
        private IRecipeRepository _repository;

        public ManageApprovalController (IRecipeRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            ListRecipeViewModel vm = new ListRecipeViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["Recipes-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));
         
            var feedback = _repository.GetAllUnapprovedRecipe(pageInfo);

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

        public ActionResult ApprovedRecipesIndex()
        {
            ListRecipeViewModel vm = new ListRecipeViewModel();

            // Create PageInfo
            PageInfo pageInfo;
            var page = Request.QueryString["Recipes-page"];
            pageInfo = page == null ? new PageInfo(AppConfigs.DefaultAdminGridPageSize, 1) : new PageInfo(AppConfigs.DefaultAdminGridPageSize, int.Parse(page));

            var feedback = _repository.GetAllApprovedRecipe(pageInfo);

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

        public ActionResult Approve(int id)
        {
            var feedback = _repository.ApproveRecipe(id);

            if (feedback.Success)
            {
                SetMessage(Messages.ApproveRecipeSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Unapprove(int id)
        {
            var feedback = _repository.UnapproveRecipe(id);

            if (feedback.Success)
            {
                SetMessage(Messages.ApproveRecipeSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ApprovedRecipesIndex");
        }

    }
}
