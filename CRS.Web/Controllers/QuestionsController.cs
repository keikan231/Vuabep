using System.Collections.Generic;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Resources;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;
using CRS.Web.ViewModels.Questions;
using CRS.Web.Framework.Security;

namespace CRS.Web.Controllers
{
    /// <summary>
    /// Handles Q&A pages (questions)
    /// </summary>
    public class QuestionsController : FrontEndControllerBase
    {
        private readonly IQuestionRepository _qRepo;
        private readonly IAnswerRepository _aRepo;

        public QuestionsController(IQuestionRepository repo1, IAnswerRepository repo2)
        {
            _qRepo = repo1;
            _aRepo = repo2;
        }

        #region List Question

        public ActionResult Index(int page = 1)
        {
            ListQuestionViewModel vm = null;
            var pageInfo = new PageInfo(AppConfigs.QuestionsPageSize, page);
            var feedback = _qRepo.GetAllQuestions(pageInfo);
            if (feedback.Success)
            {
                vm = new ListQuestionViewModel
                {
                    Questions = feedback.Questions,
                    Page = pageInfo.PageNo,
                    HasMore = feedback.Total > pageInfo.PageSize * pageInfo.PageNo,
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View();
        }

        [HttpPost]
        public ActionResult IndexMore(int page = 1)
        {
            ListQuestionViewModel vm = null;
            var pageInfo = new PageInfo(AppConfigs.QuestionsPageSize, page);
            var feedback = _qRepo.GetAllQuestions(pageInfo);
            if (feedback.Success)
            {
                vm = new ListQuestionViewModel
                {
                    Questions = feedback.Questions,
                    Page = pageInfo.PageNo,
                    HasMore = feedback.Total > pageInfo.PageSize * pageInfo.PageNo,
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        #endregion

        #region Question Details

        public ActionResult Details(int id)
        {
            ViewQuestionDetailsViewModel vm = new ViewQuestionDetailsViewModel { HighlightAnswers = new List<Answer>() };
            var pageInfo = new PageInfo(AppConfigs.AnswersPageSize, 1);

            var feedback = _qRepo.GetQuestionDetails(id);
            var feedback2 = _aRepo.GetAllAnswers(id, pageInfo);
            var feedback3 = _aRepo.GetHighlightAnswers(id, AppConfigs.HighlightAnswerNumber);

            if (feedback.Success && feedback2.Success && feedback3.Success)
            {
                vm = new ViewQuestionDetailsViewModel
                {
                    Question = feedback.Data,
                    Answers = feedback2.Answers,
                    Total = feedback2.Total,
                    HasMore = feedback2.Total > pageInfo.PageSize * pageInfo.PageNo,
                    CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0,
                    HighlightAnswers = feedback3.Data,
                    CanEdit = CurrentUser != null ? SecurityHelper.CanEditContentsDirectly() : false,
                };
                return View(vm);
            }
            return View("NotFound");
        }

        [HttpPost]
        public ActionResult DetailsMore(int id, int page = 1)
        {
            ListAnswerViewModel vm = null;
            var pageInfo = new PageInfo(AppConfigs.AnswersPageSize, page);
            var feedback = _aRepo.GetAllAnswers(id, pageInfo);
            if (feedback.Success)
            {
                vm = new ListAnswerViewModel
                {
                    Answers = feedback.Answers,
                    HasMore = feedback.Total > pageInfo.PageSize * pageInfo.PageNo,
                    CurrentUserId = CurrentUser != null ? CurrentUser.UserInfo.Id : 0,                   
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        #endregion

        #region Insert/Update

        public ViewResult Create()
        {
            return View(new InsertQuestionViewModel { Question = new Question() });
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Create(InsertQuestionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Question.TitleUrl = vm.Question.Title.ToUrlFriendly();
                vm.Question.PostedById = CurrentUser.UserInfo.Id;

                var feedback = _qRepo.InsertQuestion(vm.Question);

                if (feedback.Success)
                {
                    return RedirectToAction("Details", new { id = feedback.Data.Id, questionTitleUrl = feedback.Data.TitleUrl });
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [PermissionAuthorize]
        public ViewResult Edit(int id)
        {
            InsertQuestionViewModel vm = null;
            var feedback = _qRepo.GetQuestionForEditing(id);
            if (feedback.Success)
            {
                if (feedback.Data.PostedById != CurrentUser.UserInfo.Id && !SecurityHelper.CanEditContentsDirectly())
                {
                    SetMessage(Messages.EditQuestion_NotAllowed, MessageType.Error);
                    return View(vm);
                }

                vm = new InsertQuestionViewModel
                {
                    Question = feedback.Data,
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Edit(InsertQuestionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Question.UpdatedById = CurrentUser.UserInfo.Id;

                var feedback = _qRepo.UpdateQuestion(vm.Question);

                if (feedback.Success)
                {
                    return RedirectToAction("Details", new { id = feedback.Data.Id, questionTitleUrl = feedback.Data.TitleUrl });
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        #endregion

        #region Question AJAX

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ReportQuestion(int reportId, string reason)
        {
            var rq = new ReportedQuestion { QuestionId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _qRepo.ReportQuestion(rq);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

        #endregion

        #region Answer AJAX

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Reply(Answer answer)
        {
            var vm = new ListAnswerViewModel { Answers = new List<Answer>(), CurrentUserId = CurrentUser.UserInfo.Id };

            if (ModelState.IsValid)
            {
                answer.PostedById = CurrentUser.UserInfo.Id;

                // Call Repository to perform insert
                var feedback = _aRepo.InsertAnswer(answer);
                if (feedback.Success)
                {
                    // Add new category to cache
                    vm.Answers.Add(feedback.Data);

                    return View(vm);
                }
            }

            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult VoteAnswer(int id, bool isUpVote)
        {
            var va = new VotedAnswer { IsUpVote = isUpVote, AnswerId = id, VotedById = CurrentUser.UserInfo.Id };
            var feedback = _aRepo.VoteAnswer(va);

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
        public ActionResult ReportAnswer(int reportId, string reason)
        {
            var ra = new ReportedAnswer { AnswerId = reportId, Reason = reason, ReportedById = CurrentUser.UserInfo.Id };
            var feedback = _aRepo.ReportAnswer(ra);
            return Json(new Feedback(feedback.Success, feedback.Message));
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult DeleteAnswer(int id)
        {
            var a = new Answer { Id = id, UpdatedById = CurrentUser.UserInfo.Id };
            var feedback = _aRepo.DeleteAnswer(a);

            return Json(feedback);

        }

        #endregion
    }
}
