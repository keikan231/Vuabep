using System.Collections.Generic;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Web.Framework.Filters;
using CRS.Web.ViewModels.NewsComments;

namespace CRS.Web.Controllers
{
    /// <summary>
    /// Handles comment actions
    /// </summary>
    public class NewsCommentsController : FrontEndControllerBase
    {
        private readonly INewsCommentRepository _commentRepository;

        public NewsCommentsController(INewsCommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public ActionResult List(int newsId = 0, int total = 0)
        {
            var pageInfo = new PageInfo(AppConfigs.CommentsPageSize, 1);
            var vm = new ListNewsCommentViewModel { NewsId = newsId, Total = total, HighlightComments = new List<NewsComment>() };

            var feedback = _commentRepository.GetNewsComments(newsId, pageInfo);
            var feedback2 = _commentRepository.GetHighlightNewsComments(newsId, AppConfigs.HighlightCommentNumber);
            if (feedback.Success && feedback2.Success)
            {
                vm.Comments = feedback.Data;
                vm.HighlightComments = feedback2.Data;
                vm.CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0;
                vm.HasMore = vm.Total > pageInfo.PageSize * pageInfo.PageNo;
            }
            return View(vm);
        }

        public ActionResult ListMore(int newsId = 0, int total = 0, int page = 1)
        {
            var pageInfo = new PageInfo(AppConfigs.CommentsPageSize, page);
            var vm = new ListNewsCommentViewModel { NewsId = newsId, Total = total };

            var feedback = _commentRepository.GetNewsComments(newsId, pageInfo);
            if (feedback.Success)
            {
                vm.Comments = feedback.Data;
                vm.CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0;
                vm.HasMore = vm.Total > pageInfo.PageSize * pageInfo.PageNo;
            }
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Comment(NewsComment pc)
        {
            var vm = new ListNewsCommentViewModel { Comments = new List<NewsComment>(), CurrentUserId = CurrentUser.UserInfo.Id };
            if (ModelState.IsValid)
            {
                pc.PostedById = CurrentUser.UserInfo.Id;

                // Call Repository to perform insert
                var feedback = _commentRepository.InsertNewsComment(pc);
                if (feedback.Success)
                {
                    vm.Comments.Add(feedback.Data);
                    return View(vm);
                }
            }
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult DeleteComment(int id)
        {
            var a = new NewsComment { Id = id, UpdatedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.DeleteNewsComment(a);

            return Json(feedback);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult VoteComment(int id, bool isUpVote)
        {
            var vq = new VotedNewsComment { IsUpVote = isUpVote, CommentId = id, VotedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.VoteNewsComment(vq);

            var data = new Feedback<IList<int>>(feedback.Success, feedback.Message, new List<int> {-1, -1 });
            if (feedback.Success)
            {
                data.Data[0] = feedback.Data.UpVotes;
                data.Data[1] = feedback.Data.DownVotes;
            }

            return Json(data);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ReportComment(int reportId, string reason)
        {
            var rc = new ReportedNewsComment { CommentId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.ReportNewsComment(rc);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

    }
}
