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
using CRS.Web.ViewModels.Tips;

namespace CRS.Web.Controllers
{
    public class TipsController : FrontEndControllerBase
    {
        #region Variables

        private readonly ITipRepository _tipRepository;
        private readonly IUploadHandler _uploadHandler;
        private readonly IFileManager _fileManager;

        #endregion

        #region Constructor

        public TipsController(ITipRepository tipRepository, IUploadHandler uploadHandler, IFileManager fileManager)
        {
            _tipRepository = tipRepository;
            _uploadHandler = uploadHandler;
            _fileManager = fileManager;
        }

        #endregion

        #region List Tips

        public ActionResult Index(string sort, int page = 1)
        {
            ListTipViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, page);
            var feedback = _tipRepository.GetAllTip(criteria);
            if (feedback.Success)
            {
                vm = new ListTipViewModel
                {
                    Tips = feedback.Tips,
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
            ListTipViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, page);
            var feedback = _tipRepository.GetAllTip(criteria);
            if (feedback.Success)
            {
                vm = new ListTipViewModel
                {
                    Tips = feedback.Tips,
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

        public ActionResult TipCategoryIndex(int id, string sort, int page = 1)
        {
            ListTipViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, page);
            var feedback = _tipRepository.GetTipByCatogory(id, criteria);
            if (feedback.Success)
            {
                vm = new ListTipViewModel
                {
                    TipCategoryName = feedback.TipCategoryName,
                    Tips = feedback.Tips,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort,
                    TipCategoryId = id
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        public ActionResult TipCategoryIndexMore(int id, string sort, int page = 1)
        {
            ListTipViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.TipPageSize, page);
            var feedback = _tipRepository.GetTipByCatogory(id, criteria);
            if (feedback.Success)
            {
                vm = new ListTipViewModel
                {
                    TipCategoryName = feedback.TipCategoryName,
                    Tips = feedback.Tips,
                    Page = criteria.PageInfo.PageNo,
                    HasMore = feedback.Total > criteria.PageInfo.PageSize * criteria.PageInfo.PageNo,
                    OrderBy = criteria.OrderBy,
                    Sort = sort,
                    TipCategoryId = id
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        #endregion

        #region Tip Details

        public ActionResult Details(int id)
        {
            TipDetailsViewModel vm = new TipDetailsViewModel();

            var feedback = _tipRepository.GetTipDetails(id);

            if (feedback.Success)
            {
                vm = new TipDetailsViewModel
                {
                    Tip = feedback.Data,
                    CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0,
                    CanEdit = CurrentUser != null ? SecurityHelper.CanEditContentsDirectly() : false
                };
                return View(vm);
            }
            return View("NotFound");
        }

        #endregion

        #region Insert/Update

        public ViewResult Create()
        {
            InsertTipViewModel vm = PrepareTipCategories(null);

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Create(InsertTipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Tip.ContentHtml = vm.Tip.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.Tip.ContentHtml.RemoveHtml() == string.Empty)
                {
                    vm = PrepareTipCategories(vm);
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                vm.Tip.ImageUrl = TipImageUrl(vm);
                if (vm.Tip.ImageUrl == null)
                {
                    vm = PrepareTipCategories(vm);
                    return View(vm);
                }

                vm.Tip.TitleUrl = vm.Tip.Title.ToUrlFriendly();
                vm.Tip.PostedById = CurrentUser.UserInfo.Id;

                var feedback = _tipRepository.InsertTip(vm.Tip);

                if (feedback.Success)
                {
                    bool isPointChanged = CurrentUser.UserInfo.Point != feedback.NewPoint;
                    CurrentUser.UserInfo.Point = feedback.NewPoint;
                    if (isPointChanged)
                    {
                        SetMessage(string.Format("Cảm ơn bạn đã đóng góp mẹo vặt mới. Bạn đã được cộng thêm {0} điểm.", ReferenceDataCache.PointConfigCollection.CreateTip), MessageType.Success);
                    }
                    else
                    {
                        SetMessage("Cảm ơn bạn đã đóng góp mẹo vặt mới. Tuy nhiên bạn không được cộng thêm điểm vì bạn đã đạt số điểm tối đa cho phép một ngày.", MessageType.Info);
                    }

                    return RedirectToAction("Details", new { id = feedback.Tip.Id, tipTitleUrl = feedback.Tip.TitleUrl });
                }

                // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.Tip.ImageUrl))
                {
                    _fileManager.DeleteSingleThumbnailImage(vm.Tip.ImageUrl);
                    vm.Tip.ImageUrl = null;
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            vm = PrepareTipCategories(vm);

            return View(vm);
        }

        [PermissionAuthorize]
        public ViewResult Edit(int id)
        {
            InsertTipViewModel vm = null;
            var feedback = _tipRepository.GetTipForEditing(id);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditTip_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertTipViewModel
                {
                    Tip = feedback.Data,
                };
                vm = PrepareTipCategories(vm);
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Edit(InsertTipViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Tip.ContentHtml = vm.Tip.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.Tip.ContentHtml.RemoveHtml() == string.Empty)
                {
                    vm = PrepareTipCategories(vm);
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                if (vm.Tip.ImageUrl == null)
                {
                    vm.Tip.ImageUrl = TipImageUrl(vm);
                }

                if (vm.Tip.ImageUrl == null)
                {
                    vm = PrepareTipCategories(vm);
                    return View(vm);
                }

                vm.Tip.UpdatedById = CurrentUser.UserInfo.Id;

                var feedback = _tipRepository.UpdateTip(vm.Tip);

                if (feedback.Success)
                {
                    if (vm.OldImageUrl != null)
                    {
                        _fileManager.DeleteSingleThumbnailImage(vm.OldImageUrl);
                    }
                    return RedirectToAction("Details", new { id = feedback.Data.Id, tipTitleUrl = feedback.Data.TitleUrl });
                }

                    // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.Tip.ImageUrl))
                {
                    _fileManager.DeleteSingleThumbnailImage(vm.Tip.ImageUrl);
                    vm.Tip.ImageUrl = null;
                }

                SetMessage(feedback.Message, MessageType.Error);
            }

            vm = PrepareTipCategories(vm);
            return View(vm);
        }

        [PermissionAuthorize]
        public ActionResult DeleteImage(int tipId = 0)
        {
            InsertTipViewModel vm = null;
            var feedback = _tipRepository.GetTipForEditing(tipId);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditNews_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertTipViewModel
                {
                    Tip = feedback.Data,
                };
            }

            if (vm != null)
            {
                vm.OldImageUrl = vm.Tip.ImageUrl;
                vm.Tip.ImageUrl = null;
            }

            return PartialView("_UploadImage", vm);
        }

        #endregion

        #region Tips AJAX

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ReportTip(int reportId, string reason)
        {
            var rt = new ReportedTip { TipId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _tipRepository.ReportTip(rt);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult VoteTip(int id, bool isUpVote)
        {
            var vq = new VotedTip { IsUpVote = isUpVote, TipId = id, VotedById = CurrentUser.UserInfo.Id };
            var feedback = _tipRepository.VoteTip(vq);

            var data = new Feedback<IList<int>>(feedback.Success, feedback.Message, new List<int> { -1, -1 });
            if (feedback.Success)
            {
                data.Data[0] = feedback.Data.UpVotes;
                data.Data[1] = feedback.Data.DownVotes;
            }

            return Json(data);
        }

        #endregion

        #region Private method

        private InsertTipViewModel PrepareTipCategories(InsertTipViewModel viewModel)
        {
            var tipCategories = ReferenceDataCache.TipCategoryCollection.ToList();

            if (viewModel == null)
            {
                viewModel = new InsertTipViewModel
                                {
                                    Tip = new Tip(),
                                    TipCategories = tipCategories
                                };


            }
            else
            {
                viewModel.TipCategories = tipCategories;
            }
            return viewModel;
        }

        private string TipImageUrl(InsertTipViewModel vm)
        {
            string newUrl = null;
            if (vm.File == null)
            {
                string msg = "Ảnh đại diện là bắt buộc";
                SetMessage(msg, MessageType.Error);

                return null;
            }

            var uploadFeedback = _uploadHandler.UploadImage(vm.File);
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
            newUrl = string.Format("{0}", uploadFeedback.Data);
            return newUrl;
        }

        #endregion

    }
}
