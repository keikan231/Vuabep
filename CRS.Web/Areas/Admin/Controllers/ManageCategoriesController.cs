using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Caching;
using CRS.Common.Helpers;
using CRS.Common.Logging;
using CRS.Resources;
using CRS.Web.Areas.Admin.ViewModels.ManageCategories;
using CRS.Web.Controllers;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles categories management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageCategories)]
    public class ManageCategoriesController : AdminControllerBase
    {
        private ITipCategoryRepository _repository;
        private IRecipeCategoryRepository _recipeRepository;
        private IRecipeSmallCategoryRepository _recipeSmallRepository;

        public ManageCategoriesController(ITipCategoryRepository repository, IRecipeCategoryRepository recipeRepository, IRecipeSmallCategoryRepository recipeSmallRepository)
        {
            _repository = repository;
            _recipeRepository = recipeRepository;
            _recipeSmallRepository = recipeSmallRepository;
        }

        #region Tip Categories
        public ViewResult ManageTipCategories()
        {
            ListTipCategoryViewModel vm = new ListTipCategoryViewModel();
            vm.TipCategories = ReferenceDataCache.TipCategoryCollection;

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteTipCategory(int id)
        {
            Feedback feedback = _repository.DeleteTipCategory(id);
            if (feedback.Success)
            {
                // Remove from cache
                ReferenceDataCache.TipCategoryCollection.RemoveAll(i => i.Id == id);

                SetMessage("Tip Category has been deleted successfully", MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ViewResult CreateTipCategory()
        {
            return View(new InsertTipCategoryViewModel { TipCategory = new TipCategory() });
        }

        [HttpPost]
        public ActionResult CreateTipCategory(InsertTipCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.TipCategory.NameUrl = vm.TipCategory.Name.ToUrlFriendly();

                // Call Repository to perform insert
                Feedback<TipCategory> feedback = _repository.InsertTipCategory(vm.TipCategory);

                if (feedback.Success)
                {
                    // Add new TipCategory to cache
                    TipCategory exist = ReferenceDataCache.TipCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist == null)
                    {
                        ReferenceDataCache.TipCategoryCollection.Add(feedback.Data);
                        ReferenceDataCache.TipCategoryCollection.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
                    }

                    SetMessage("Tip Category has been inserted successfully", MessageType.Success);

                    return RedirectToAction("ManageTipCategories");
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }

            return View(vm);
        }

        public ActionResult EditTipCategory(int id = 0)
        {
            var feedback = _repository.GetTipCategoryDetails(id);
            if (feedback.Success)
            {
                return View(new InsertTipCategoryViewModel { TipCategory = feedback.Data });
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
                return RedirectToAction("ManageTipCategories");
            }
        }

        [HttpPost]
        public ActionResult EditTipCategory(InsertTipCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.TipCategory.NameUrl = vm.TipCategory.Name.ToUrlFriendly();

                // Call repository to perform insert
                Feedback<TipCategory> feedback = _repository.UpdateTipCategory(vm.TipCategory);
                if (feedback.Success)
                {
                    // Replace current TipCategory in cache by updated one
                    TipCategory exist = ReferenceDataCache.TipCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist != null)
                    {
                        ReferenceDataCache.TipCategoryCollection.Remove(exist);
                        ReferenceDataCache.TipCategoryCollection.Add(feedback.Data);
                        ReferenceDataCache.TipCategoryCollection.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
                    }

                    SetMessage("Tip Category has been updated successfully", MessageType.Success);

                    return RedirectToAction("ManageTipCategories");
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }

            return View(vm);
        }

        #endregion

        #region Recipe Categories

        public ViewResult ManageRecipeCategories()
        {
            ListRecipeCategoryViewModel vm = new ListRecipeCategoryViewModel();
            vm.RecipeCategories = ReferenceDataCache.RecipeCategoryCollection;

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteRecipeCategory(int id)
        {
            Feedback feedback = _recipeRepository.DeleteRecipeCategory(id);
            if (feedback.Success)
            {
                // Remove from cache
                ReferenceDataCache.RecipeCategoryCollection.RemoveAll(i => i.Id == id);

                SetMessage("Recipe Category has been deleted successfully", MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ViewResult CreateRecipeCategory()
        {
            return View(new InsertRecipeCategoryViewModel { RecipeCategory = new RecipeCategory() });
        }

        [HttpPost]
        public ActionResult CreateRecipeCategory(InsertRecipeCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.RecipeCategory.NameUrl = vm.RecipeCategory.Name.ToUrlFriendly();

                // Call Repository to perform insert
                Feedback<RecipeCategory> feedback = _recipeRepository.InsertRecipeCategory(vm.RecipeCategory);

                if (feedback.Success)
                {
                    // Add new RecipeCategory to cache
                    RecipeCategory exist = ReferenceDataCache.RecipeCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist == null)
                    {
                        ReferenceDataCache.RecipeCategoryCollection.Add(feedback.Data);
                        ReferenceDataCache.RecipeCategoryCollection.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
                    }

                    SetMessage("Recipe Category has been inserted successfully", MessageType.Success);

                    return RedirectToAction("ManageRecipeCategories");
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }

            return View(vm);
        }

        public ActionResult EditRecipeCategory(int id = 0)
        {
            var feedback = _recipeRepository.GetRecipeCategoryDetails(id);
            if (feedback.Success)
            {
                return View(new InsertRecipeCategoryViewModel { RecipeCategory = feedback.Data });
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
                return RedirectToAction("ManageRecipeCategories");
            }
        }

        [HttpPost]
        public ActionResult EditRecipeCategory(InsertRecipeCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.RecipeCategory.NameUrl = vm.RecipeCategory.Name.ToUrlFriendly();

                // Call repository to perform insert
                Feedback<RecipeCategory> feedback = _recipeRepository.UpdateRecipeCategory(vm.RecipeCategory);
                if (feedback.Success)
                {
                    // Replace current TipCategory in cache by updated one
                    RecipeCategory exist = ReferenceDataCache.RecipeCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist != null)
                    {
                        ReferenceDataCache.RecipeCategoryCollection.Remove(exist);
                        ReferenceDataCache.RecipeCategoryCollection.Add(feedback.Data);
                        ReferenceDataCache.RecipeCategoryCollection.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
                    }

                    SetMessage("Recipe Category has been updated successfully", MessageType.Success);

                    return RedirectToAction("ManageRecipeCategories");
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }

            return View(vm);
        }

        #endregion

        #region Recipe Small Categories

        public ViewResult ManageRecipeSmallCategories()
        {
            ListRecipeSmallCategoryViewModel vm = new ListRecipeSmallCategoryViewModel();
            vm.RecipeSmallCategories = ReferenceDataCache.RecipeSmallCategoryCollection;

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteRecipeSmallCategory(int id)
        {
            Feedback feedback = _recipeSmallRepository.DeleteRecipeSmallCategory(id);
            if (feedback.Success)
            {
                // Remove from cache
                ReferenceDataCache.RecipeSmallCategoryCollection.RemoveAll(i => i.Id == id);

                SetMessage("Small Recipe Category has been deleted successfully", MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ViewResult CreateRecipeSmallCategory()
        {
            var viewModel = new InsertRecipeSmallCategoryViewModel { RecipeSmallCategory = new RecipeSmallCategory(), RecipeCategory = new List<CheckedRecipeCategory>(), TipCategories = PrepareTipCategories()};
            foreach (var recipeCategory in ReferenceDataCache.RecipeCategoryCollection)
            {
                viewModel.RecipeCategory.Add(new CheckedRecipeCategory { RecipeCategory = recipeCategory });
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateRecipeSmallCategory(InsertRecipeSmallCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.RecipeSmallCategory.NameUrl = vm.RecipeSmallCategory.Name.ToUrlFriendly();

                IList<int> c = (from t in vm.RecipeCategory where t.IsChecked select t.RecipeCategory.Id).ToList();

                // Call Repository to perform insert
                Feedback<RecipeSmallCategory> feedback =
                    _recipeSmallRepository.InsertRecipeSmallCategory(vm.RecipeSmallCategory, c);

                if (feedback.Success)
                {
                    // Add new TipCategory to cache
                    RecipeSmallCategory exist = ReferenceDataCache.RecipeSmallCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist == null)
                    {
                        // Remove Collection from cache to force reload on next access
                        Cacher.Remove(Constants.ConfigKeys.Caching.RecipeSmallCategoryCollectionCacheMinutes);
                        Cacher.Remove(Constants.ConfigKeys.Caching.RecipeAllCategoryCollectionCacheMinutes);
                    }
                    SetMessage("Small Recipe Category has been inserted successfully", MessageType.Success);

                    return RedirectToAction("ManageRecipeSmallCategories");
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }
            vm.TipCategories = PrepareTipCategories();
            return View(vm);
        }

        public ActionResult EditRecipeSmallCategory(int id = 0)
        {
            var feedback = _recipeSmallRepository.GetRecipeSmallCategoryDetails(id);
            if (feedback.Success)
            {
                IList<CheckedRecipeCategory> checkedRecipeCategories = new List<CheckedRecipeCategory>();
                foreach (RecipeCategory t in ReferenceDataCache.RecipeCategoryCollection)
                {
                    CheckedRecipeCategory checkedRecipeCategory = new CheckedRecipeCategory();
                    checkedRecipeCategory.RecipeCategory = t;
                    if (feedback.Data.RecipeCategoryMappings.Any(i => i.RecipeCategoryId == t.Id))
                        checkedRecipeCategory.IsChecked = true;

                    checkedRecipeCategories.Add(checkedRecipeCategory);
                }
                return View(new InsertRecipeSmallCategoryViewModel { RecipeSmallCategory = feedback.Data, RecipeCategory = checkedRecipeCategories, TipCategories = PrepareTipCategories()});
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
                return RedirectToAction("ManageRecipeCategories");
            }
        }

        [HttpPost]
        public ActionResult EditRecipeSmallCategory(InsertRecipeSmallCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Generate Url friendly name
                vm.RecipeSmallCategory.NameUrl = vm.RecipeSmallCategory.Name.ToUrlFriendly();

                IList<int> c = (from t in vm.RecipeCategory where t.IsChecked select t.RecipeCategory.Id).ToList();

                // Call repository to perform insert
                Feedback<RecipeSmallCategory> feedback =
                    _recipeSmallRepository.UpdateRecipeSmallCategory(vm.RecipeSmallCategory, c);
                if (feedback.Success)
                {
                    // Replace current TipCategory in cache by updated one
                    RecipeSmallCategory exist =
                        ReferenceDataCache.RecipeSmallCategoryCollection.FirstOrDefault(i => i.Id == feedback.Data.Id);
                    if (exist != null)
                    {
                        // Remove Collection from cache to force reload on next access
                        Cacher.Remove(Constants.ConfigKeys.Caching.RecipeSmallCategoryCollectionCacheMinutes);
                        Cacher.Remove(Constants.ConfigKeys.Caching.RecipeAllCategoryCollectionCacheMinutes);

                        SetMessage("Small Recipe Category has been updated successfully", MessageType.Success);

                        return RedirectToAction("ManageRecipeSmallCategories");
                    }
                }
                else
                {
                    SetMessage(feedback.Message, MessageType.Error);
                }
            }
            vm.TipCategories = PrepareTipCategories();
            return View(vm);
        }

        #endregion

        #region Private method

        private IList<TipCategory> PrepareTipCategories()
        {
            var categories = ReferenceDataCache.TipCategoryCollection.ToList();
            return categories;
        }

        #endregion
    }
}
