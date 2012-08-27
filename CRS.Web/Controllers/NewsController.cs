using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Business.DataSorting;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Resources;
using CRS.Web.Framework.Filters;
using CRS.Web.Framework.Security;
using CRS.Web.Models;
using CRS.Web.Models.FileManagement;
using CRS.Web.ViewModels._News;

namespace CRS.Web.Controllers
{
    public class NewsController : FrontEndControllerBase
    {
        //
        // GET: /News/

        #region Variables

        private readonly INewsRepository _newsRepository;
        private readonly IUploadHandler _uploadHandler;
        private readonly IFileManager _fileManager;

        #endregion

        #region Constructor

        public NewsController(INewsRepository newsRepository, IUploadHandler uploadHandler, IFileManager fileManager)
        {
            _newsRepository = newsRepository;
            _uploadHandler = uploadHandler;
            _fileManager = fileManager;
        }

        #endregion

        #region List News

        public ActionResult Index(string sort, int page = 1)
        {
            ListNewsViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.NewsPageSize, page);
            var feedback = _newsRepository.GetAllNews(criteria);
            if (feedback.Success)
            {
                vm = new ListNewsViewModel
                {
                    News = feedback.News,
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
            ListNewsViewModel vm = null;
            SortCriteria criteria = new SortCriteria();
            criteria.OrderBy = Sort.GetSortKey(sort);
            criteria.PageInfo = new PageInfo(AppConfigs.NewsPageSize, page);
            var feedback = _newsRepository.GetAllNews(criteria);
            if (feedback.Success)
            {
                vm = new ListNewsViewModel
                {
                    News = feedback.News,
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

        #region News Details

        public ActionResult Details(int id)
        {

            NewsDetailsViewModel vm = new NewsDetailsViewModel();

            var feedback = _newsRepository.GetNewsDetails(id);

            if (feedback.Success)
            {
                vm = new NewsDetailsViewModel
                {
                    News = feedback.Data,
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
            return View(new InsertNewsViewModel { News = new News() });
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Create(InsertNewsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.News.ContentHtml = vm.News.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.News.ContentHtml.RemoveHtml() == string.Empty)
                {
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                vm.News.ImageUrl = NewImageUrl(vm);

                if (vm.News.ImageUrl == null)
                {
                    return View(vm);
                }

                vm.News.TitleUrl = vm.News.Title.ToUrlFriendly();
                vm.News.PostedById = CurrentUser.UserInfo.Id;

                var feedback = _newsRepository.InsertNews(vm.News);

                if (feedback.Success)
                {
                    bool isPointChanged = CurrentUser.UserInfo.Point != feedback.NewPoint;
                    CurrentUser.UserInfo.Point = feedback.NewPoint;
                    
                    if(isPointChanged)
                    {
                        SetMessage(string.Format("Cảm ơn bạn đã đóng góp tin tức mới. Bạn đã được cộng thêm {0} điểm.", ReferenceDataCache.PointConfigCollection.CreateNews), MessageType.Success);
                    }
                    else
                    {
                        SetMessage("Cảm ơn bạn đã đóng góp tin tức mới. Tuy nhiên bạn không được cộng thêm điểm vì bạn đã đạt số điểm tối đa cho phép một ngày.", MessageType.Info);
                    }

                    return RedirectToAction("Details", new { id = feedback.News.Id, newsTitleUrl = feedback.News.TitleUrl });
                }

                // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.News.ImageUrl))
                {
                    _fileManager.DeleteSingleThumbnailImage(vm.News.ImageUrl);
                    vm.News.ImageUrl = null;
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [PermissionAuthorize]
        public ViewResult Edit(int id)
        {
            InsertNewsViewModel vm = null;
            var feedback = _newsRepository.GetNewsForEditing(id);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditNews_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertNewsViewModel
                {
                    News = feedback.Data,
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Edit(InsertNewsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.News.ContentHtml = vm.News.ContentHtml.GetSafeHtml(Constants.AllowedHtmlTags, Constants.AllowedHtmlAttributes);
                if (vm.News.ContentHtml.RemoveHtml() == string.Empty)
                {
                    SetMessage(Messages.ContentIsEmpty, MessageType.Error);
                    return View(vm);
                }

                if (vm.News.ImageUrl == null)
                {
                    vm.News.ImageUrl = NewImageUrl(vm);
                }

                if (vm.News.ImageUrl == null)
                {
                    return View(vm);
                }

                vm.News.UpdatedById = CurrentUser.UserInfo.Id;

                var feedback = _newsRepository.UpdateNews(vm.News);

                if (feedback.Success)
                {
                    if (vm.OldImageUrl != null)
                    {
                        _fileManager.DeleteSingleThumbnailImage(vm.OldImageUrl);
                    }
                    return RedirectToAction("Details", new { id = feedback.Data.Id, newsTitleUrl = feedback.Data.TitleUrl });
                }
                // If update to DB unsuccessfully, delete new image
                if (!string.IsNullOrEmpty(vm.News.ImageUrl))
                {
                    _fileManager.DeleteSingleThumbnailImage(vm.News.ImageUrl);
                    vm.News.ImageUrl = null;
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [PermissionAuthorize]
        public ActionResult DeleteImage(int newsId = 0)
        {
            InsertNewsViewModel vm = null;
            var feedback = _newsRepository.GetNewsForEditing(newsId);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditNews_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertNewsViewModel
                         {
                             News = feedback.Data,
                         };
            }

            if (vm != null)
            {
                vm.OldImageUrl = vm.News.ImageUrl;
                vm.News.ImageUrl = null;
            }

            return PartialView("_UploadImage", vm);
        }

        #endregion

        #region News AJAX

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ReportNews(int reportId, string reason)
        {
            var rn = new ReportedNews { NewsId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _newsRepository.ReportNews(rn);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

        #endregion

        #region Private method

        private string NewImageUrl(InsertNewsViewModel vm)
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
                SetMessage(msg, MessageType.Error);

                return null;
            }

            newUrl = string.Format("{0}", uploadFeedback.Data);
            return newUrl;
        }

        #endregion
    }
}
