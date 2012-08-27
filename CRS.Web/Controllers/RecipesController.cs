using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRS.Business.DataSorting;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Resources;
using CRS.Web.Framework.Filters;
using CRS.Web.Framework.Security;
using CRS.Web.Models;
using CRS.Web.Models.FileManagement;
using CRS.Web.ViewModels.Recipes;

namespace CRS.Web.Controllers
{
    public class RecipesController : FrontEndControllerBase
    {
         #region Variables

        private readonly IRecipeRepository _recipeRepository;
        private readonly IUploadHandler _uploadHandler;
        private readonly IFileManager _fileManager;

        #endregion

        #region Constructor

        public RecipesController(IRecipeRepository recipeRepository, IUploadHandler uploadHandler, IFileManager fileManager)
        {
            _recipeRepository = recipeRepository;
            _uploadHandler = uploadHandler;
            _fileManager = fileManager;
        }

        #endregion

        #region List Recipes

        public ActionResult Index(string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetAllRecipe(criteria);
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                {
                    Recipes = feedback.Recipes,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        public ActionResult IndexMore(string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetAllRecipe(criteria);
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                {
                    Recipes = feedback.Recipes,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        public ActionResult RecipeCategoryIndex(int id,string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetRecipeByCatogory(id, criteria, "big");
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                {
                    Categories = feedback.Categories,
                    Recipes = feedback.Recipes,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RecipeCategoryIndexMore(int id, string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetRecipeByCatogory(id, criteria, "big");
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                {
                    Categories = feedback.Categories,
                    Recipes = feedback.Recipes,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        public ActionResult RecipeSmallCategoryIndex(int id, int smallId, string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetRecipeByCatogory(id, criteria, "small", smallId);
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                         {
                             Categories = feedback.Categories,
                             Recipes = feedback.Recipes,
                             Page = criteria.PageInfo.PageNo,
                             HasMore = feedback.Total > criteria.PageInfo.PageSize*criteria.PageInfo.PageNo,
                             OrderBy = criteria.OrderBy,
                             Sort = sort
                         };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RecipeSmallCategoryIndexMore(int id, int smallId, string sort, int page = 1)
        {
            ListRecipeViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.RecipePageSize, page);
            var feedback = _recipeRepository.GetRecipeByCatogory(id, criteria, "small", smallId);
            if (feedback.Success)
            {
                vm = new ListRecipeViewModel
                {
                    Categories = feedback.Categories,
                    Recipes = feedback.Recipes,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        #endregion

        #region Recipe Details

        public ActionResult Details(int id)
        {
            RecipeDetailsViewModel vm = new RecipeDetailsViewModel();

            var feedback = _recipeRepository.GetRecipeDetails(id);

            if (feedback.Success)
            {
                vm = new RecipeDetailsViewModel
                {
                    Recipe = feedback.Recipe,
                    Categories = feedback.Categories,
                    CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0,
                    CanEdit = CurrentUser != null ? SecurityHelper.CanEditContentsDirectly() : false
                };
                return View(vm);
            }
            return View("NotFound");
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Rate(RatedRecipe ratedRecipe)
        {
            if (ModelState.IsValid)
            {
                ratedRecipe.RatedById = CurrentUser.UserInfo.Id;
                var feedback = _recipeRepository.RateRecipe(ratedRecipe);
                if (feedback.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = string.Format("{0} điểm / {1} lượt đánh giá",
                                                Math.Round((double)feedback.Data.TotalRates /
                                                    feedback.Data.RateTimes, 2), feedback.Data.RateTimes)
                    });
                }
                else
                {
                    return Json(new { Success = false, feedback.Message });
                }
            }

            return Json(new { Success = false, Message = "Dữ liệu không đúng." });
        }

        #endregion

        #region Insert/Update

        public ViewResult Create()
        {
            InsertRecipeViewModel vm = PrepareRecipeCategories(null);

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Create(InsertRecipeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Recipe.ContentHtml = vm.Recipe.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.Recipe.ContentHtml.RemoveHtml() == string.Empty)
                {
                    vm = PrepareRecipeCategories(vm);
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                vm.Recipe.ImageUrl = RecipeImageUrl(vm);
                if (vm.Recipe.ImageUrl == null)
                {
                    vm = PrepareRecipeCategories(vm);
                    return View(vm);
                }

                vm.Recipe.TitleUrl = vm.Recipe.Title.ToUrlFriendly();
                vm.Recipe.PostedById = CurrentUser.UserInfo.Id;

                var feedback = _recipeRepository.InsertRecipe(vm.Recipe);

                if (feedback.Success)
                {
                    bool isPointChanged = CurrentUser.UserInfo.Point != feedback.NewPoint;
                    CurrentUser.UserInfo.Point = feedback.NewPoint;
                    if (isPointChanged)
                    {
                        SetMessage(string.Format("Cảm ơn bạn đã đóng góp công thức nấu ăn mới. Bạn đã được cộng thêm {0} điểm.", ReferenceDataCache.PointConfigCollection.CreateRecipe), MessageType.Success);
                    }
                    else
                    {
                        SetMessage("Cảm ơn bạn đã đóng góp công thức nấu ăn mới. Tuy nhiên bạn không được cộng thêm điểm vì bạn đã đạt số điểm tối đa cho phép một ngày.", MessageType.Info);
                    }

                    return RedirectToAction("Details", new { id = feedback.Recipe.Id, RecipeTitleUrl = feedback.Recipe.TitleUrl });
                }

                // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.Recipe.ImageUrl))
                {
                    _fileManager.DeleteImagePair(vm.Recipe.ImageUrl);
                    vm.Recipe.ImageUrl = null;
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            vm = PrepareRecipeCategories(vm);

            return View(vm);
        }

        [PermissionAuthorize]
        public ViewResult Edit(int id)
        {
            InsertRecipeViewModel vm = null;
            var feedback = _recipeRepository.GetRecipeForEditing(id);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditRecipe_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertRecipeViewModel
                {
                    Recipe = feedback.Data,
                };
                vm = PrepareRecipeCategories(vm);
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Edit(InsertRecipeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Recipe.ContentHtml = vm.Recipe.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.Recipe.ContentHtml.RemoveHtml() == string.Empty)
                {
                    vm = PrepareRecipeCategories(vm);
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                if (vm.Recipe.ImageUrl == null)
                {
                    vm.Recipe.ImageUrl = RecipeImageUrl(vm);
                }

                if (vm.Recipe.ImageUrl == null)
                {
                    vm = PrepareRecipeCategories(vm);
                    return View(vm);
                }

                vm.Recipe.UpdatedById = CurrentUser.UserInfo.Id;

                var feedback = _recipeRepository.UpdateRecipe(vm.Recipe);

                if (feedback.Success)
                {
                    if (vm.OldImageUrl != null)
                    {
                        _fileManager.DeleteSingleThumbnailImage(vm.OldImageUrl);
                    }
                    return RedirectToAction("Details", new { id = feedback.Data.Id, RecipeTitleUrl = feedback.Data.TitleUrl });
                }

                // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.Recipe.ImageUrl))
                {
                    _fileManager.DeleteImagePair(vm.Recipe.ImageUrl);
                    vm.Recipe.ImageUrl = null;
                }

                SetMessage(feedback.Message, MessageType.Error);
            }

            vm = PrepareRecipeCategories(vm);
            return View(vm);
        }

        [PermissionAuthorize]
        public ActionResult DeleteImage(int recipeId = 0)
        {
            InsertRecipeViewModel vm = null;
            var feedback = _recipeRepository.GetRecipeForEditing(recipeId);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditNews_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertRecipeViewModel
                {
                    Recipe = feedback.Data,
                };
            }

            if (vm != null)
            {
                vm.OldImageUrl = vm.Recipe.ImageUrl;
                vm.Recipe.ImageUrl = null;
            }

            return PartialView("_UploadImage", vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult AddToFavorite(FavoriteRecipe favoriteRecipe)
        {
            favoriteRecipe.UserId = CurrentUser.UserInfo.Id;
            var feedback = _recipeRepository.AddToFavorite(favoriteRecipe);

            return Json(feedback);
        }

        #endregion

        #region Recipes AJAX

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ReportRecipe(int reportId, string reason)
        {
            var rt = new ReportedRecipe { RecipeId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _recipeRepository.ReportRecipe(rt);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

        #endregion

        #region Private method

        private InsertRecipeViewModel PrepareRecipeCategories(InsertRecipeViewModel viewModel)
        {
            var recipeAllCategories = ReferenceDataCache.RecipeAllCategoryCollection.ToList();
            var recipeCategories = ReferenceDataCache.RecipeCategoryCollection.ToList();

            IDictionary<string, IEnumerable<SelectListItem>> categories = new Dictionary<string, IEnumerable<SelectListItem>>();

            foreach (var recipeCategory in recipeCategories)
            {
                var smallCategoryList = new List<SelectListItem>();

                foreach (var allCategory in recipeAllCategories.Where(c => c.RecipeCategoryId == recipeCategory.Id))
                {
                    smallCategoryList.Add(new SelectListItem { Value = allCategory.Id.ToString(), Text = allCategory.RecipeSmallCategoryName });
                }

                categories.Add(recipeCategory.Name, smallCategoryList);
            }

            if (viewModel == null)
            {
                viewModel = new InsertRecipeViewModel
                {
                    Recipe = new Recipe(),
                    Categories = categories
                };


            }
            else
            {
                viewModel.Categories = categories;
            }
            return viewModel;
        }

        private string RecipeImageUrl(InsertRecipeViewModel vm)
        {
            string newUrl = null;
            if (vm.File == null)
            {
                string msg = "Ảnh đại diện là bắt buộc";
                SetMessage(msg, MessageType.Error);

                return null;
            }

            var uploadFeedback = _uploadHandler.UploadRecipeImage(vm.File);
            if (!uploadFeedback.Success)
            {
                string msg = uploadFeedback.Message;
                //string msg = string.Format("Chỉ hỗ trợ ảnh định dạng JPG và PNG. " +
                //      "Kích thước ảnh không được vượt quá {0}Mb. Nếu lỗi vẫn xảy ra bạn hãy thử lại vào lúc khác.",
                //      AppConfigs.UploadImageMaxSize);
                SetMessage(msg, MessageType.Error);

                return null;
            }

            //newUrl = string.Format("{0}{1}{2}", uploadFeedback.Data[0], Constants.ImageUrlsSeparator,
            //                              uploadFeedback.Data[1]);
            newUrl = string.Format("{0}{1}{2}", uploadFeedback.Data[0], Constants.ImageUrlsSeparator, uploadFeedback.Data[1]);
            return newUrl;
        }

        #endregion

    }
}
