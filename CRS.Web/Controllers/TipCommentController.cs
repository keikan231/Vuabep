using System.Collections.Generic;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Web.Framework.Filters;
using CRS.Web.ViewModels.TipComments;

namespace CRS.Web.Controllers
{
    /// <summary>
    /// Handles comment actions
    /// </summary>
    public class TipCommentsController : FrontEndControllerBase
    {
        private readonly ITipCommentRepository _commentRepository;

        public TipCommentsController(ITipCommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public ActionResult List(int tipId = 0, int total = 0)
        {
            var pageInfo = new PageInfo(AppConfigs.CommentsPageSize, 1);
            var vm = new ListTipCommentViewModel { TipId = tipId, Total = total, HighlightComments = new List<TipComment>() };

            var feedback = _commentRepository.GetTipComments(tipId, pageInfo);
            var feedback2 = _commentRepository.GetHighlightTipComments(tipId, AppConfigs.HighlightCommentNumber);
            if (feedback.Success && feedback2.Success)
            {
                vm.Comments = feedback.Data;
                vm.HighlightComments = feedback2.Data;
                vm.CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0;
                vm.HasMore = vm.Total > pageInfo.PageSize * pageInfo.PageNo;
            }
            return View(vm);
        }

        public ActionResult ListMore(int tipId = 0, int total = 0, int page = 1)
        {
            var pageInfo = new PageInfo(AppConfigs.CommentsPageSize, page);
            var vm = new ListTipCommentViewModel { TipId = tipId, Total = total };

            var feedback = _commentRepository.GetTipComments(tipId, pageInfo);
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
        public ActionResult Comment(TipComment pc)
        {
            var vm = new ListTipCommentViewModel { Comments = new List<TipComment>(), CurrentUserId = CurrentUser.UserInfo.Id };
            if (ModelState.IsValid)
            {
                pc.PostedById = CurrentUser.UserInfo.Id;

                // Call Repository to perform insert
                var feedback = _commentRepository.InsertTipComment(pc);
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
            var a = new TipComment { Id = id, UpdatedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.DeleteTipComment(a);

            return Json(feedback);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult VoteComment(int id, bool isUpVote)
        {
            var vq = new VotedTipComment { IsUpVote = isUpVote, CommentId = id, VotedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.VoteTipComment(vq);

            var data = new Feedback<IList<int>>(feedback.Success, feedback.Message, new List<int> { -1, -1 });
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
            var rc = new ReportedTipComment { CommentId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _commentRepository.ReportTipComment(rc);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

    }
}
