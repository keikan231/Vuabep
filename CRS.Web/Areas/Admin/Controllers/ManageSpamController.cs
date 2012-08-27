using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Common;
using CRS.Web.Areas.Admin.ViewModels.ManageSpam;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles spam management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageSpam)]
    public class ManageSpamController : AdminControllerBase
    {
        private INewsCommentRepository _newsCommentRepository;
        private ITipCommentRepository _tipCommentRepository;
        private IRecipeCommentRepository _recipeCommentRepository;
        private IQuestionRepository _questionRepository;
        private IAnswerRepository _answerRepository;
        private INewsRepository _newsRepository;
        private ITipRepository _tipRepository;
        private IRecipeRepository _recipeRepository;

        public ManageSpamController(INewsCommentRepository newsCommentRepository, ITipCommentRepository tipCommentRepository, IRecipeCommentRepository recipeCommentRepository, IQuestionRepository questionRepository, IAnswerRepository answerRepository, INewsRepository newsRepository, ITipRepository tipRepository, IRecipeRepository recipeRepository)
        {
            _newsCommentRepository = newsCommentRepository;
            _tipCommentRepository = tipCommentRepository;
            _recipeCommentRepository = recipeCommentRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _newsRepository = newsRepository;
            _tipRepository = tipRepository;
            _recipeRepository = recipeRepository;
        }

        public ActionResult ReportDetails(int id, string group)
        {
            if (group == "newsComment")
            {
                var vm = new ListReportedNewsCommentViewModel
                {
                    ReportedComments = _newsCommentRepository.GetReportedCommentDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedComments.Select(rc => new ReportedGeneral
                {
                    ReportedBy = rc.ReportedBy.Username,
                    ReportedDate = rc.ReportedDate,
                    Reason = rc.Reason
                }).ToList();

                return View(reports);
            }

            if (group == "tipComment")
            {
                var vm = new ListReportedTipCommentViewModel
                {
                    ReportedComments = _tipCommentRepository.GetReportedCommentDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedComments.Select(rc => new ReportedGeneral
                {
                    ReportedBy = rc.ReportedBy.Username,
                    ReportedDate = rc.ReportedDate,
                    Reason = rc.Reason
                }).ToList();

                return View(reports);
            }

            if (group == "recipeComment")
            {
                var vm = new ListReportedRecipeCommentViewModel
                {
                    ReportedComments = _recipeCommentRepository.GetReportedCommentDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedComments.Select(rc => new ReportedGeneral
                {
                    ReportedBy = rc.ReportedBy.Username,
                    ReportedDate = rc.ReportedDate,
                    Reason = rc.Reason
                }).ToList();

                return View(reports);
            }

            if (group == "question")
            {
                var vm = new ListReportedQuestionViewModel
                {
                    ReportedQuestions = _questionRepository.GetReportedQuestionDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedQuestions.Select(rp => new ReportedGeneral
                {
                    ReportedBy = rp.ReportedBy.Username,
                    ReportedDate = rp.ReportedDate,
                    Reason = rp.Reason
                }).ToList();
                return View(reports);
            }

            if (group == "answer")
            {
                var vm = new ListReportedAnswerViewModel
                {
                    ReportedAnswers = _answerRepository.GetReportedAnswerDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedAnswers.Select(rp => new ReportedGeneral
                {
                    ReportedBy = rp.ReportedBy.Username,
                    ReportedDate = rp.ReportedDate,
                    Reason = rp.Reason
                }).ToList();
                return View(reports);
            }

            if (group == "news")
            {
                var vm = new ListReportedNewsViewModel
                {
                    ReportedNews = _newsRepository.GetReportedNewsDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedNews.Select(rp => new ReportedGeneral
                {
                    ReportedBy = rp.ReportedBy.Username,
                    ReportedDate = rp.ReportedDate,
                    Reason = rp.Reason
                }).ToList();
                return View(reports);
            }

            if (group == "tip")
            {
                var vm = new ListReportedTipViewModel
                {
                    ReportedTips = _tipRepository.GetReportedTipDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedTips.Select(rp => new ReportedGeneral
                {
                    ReportedBy = rp.ReportedBy.Username,
                    ReportedDate = rp.ReportedDate,
                    Reason = rp.Reason
                }).ToList();
                return View(reports);
            }

            if (group == "recipe")
            {
                var vm = new ListReportedRecipeViewModel
                {
                    ReportedRecipes = _recipeRepository.GetReportedRecipeDetails(id).Data
                };
                IList<ReportedGeneral> reports = vm.ReportedRecipes.Select(rp => new ReportedGeneral
                {
                    ReportedBy = rp.ReportedBy.Username,
                    ReportedDate = rp.ReportedDate,
                    Reason = rp.Reason
                }).ToList();
                return View(reports);
            }

            return null;
        }

        #region Manage Report News Comments

        public ViewResult ManageReportedNewsComment(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedNewsCommentViewModel
            {
                Comments = _newsCommentRepository.GetAllReportedNewsComments(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedNewsComment(int id)
        {
            Feedback feedback = _newsCommentRepository.DeleteReportedComment(id);
            if (feedback.Success)
            {
                SetMessage("Spam Comment and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseNewsCommentReports(int id)
        {
            Feedback feedback = _newsCommentRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Comment has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Tip Comments

        public ViewResult ManageReportedTipComment(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedTipCommentViewModel
            {
                Comments = _tipCommentRepository.GetAllReportedTipComments(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedTipComment(int id)
        {
            Feedback feedback = _tipCommentRepository.DeleteReportedComment(id);
            if (feedback.Success)
            {
                SetMessage("Spam Comment and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseTipCommentReports(int id)
        {
            Feedback feedback = _tipCommentRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Comment has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Recipe Comments

        public ViewResult ManageReportedRecipeComment(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedRecipeCommentViewModel
            {
                Comments = _recipeCommentRepository.GetAllReportedRecipeComments(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedRecipeComment(int id)
        {
            Feedback feedback = _recipeCommentRepository.DeleteReportedComment(id);
            if (feedback.Success)
            {
                SetMessage("Spam Comment and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseRecipeCommentReports(int id)
        {
            Feedback feedback = _recipeCommentRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Comment has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Questions

        public ViewResult ManageReportedQuestion(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedQuestionViewModel
            {
                Questions = _questionRepository.GetAllReportedQuestion(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedQuestion(int id)
        {
            Feedback feedback = _questionRepository.DeleteReportedQuestion(id);
            if (feedback.Success)
            {
                SetMessage("Spam Question and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseQuestionReports(int id)
        {
            Feedback feedback = _questionRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Question has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Answers

        public ViewResult ManageReportedAnswer(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedAnswerViewModel
            {
                Answers = _answerRepository.GetAllReportedAnswers(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedAnswer(int id)
        {
            Feedback feedback = _answerRepository.DeleteReportedAnswer(id);
            if (feedback.Success)
            {
                SetMessage("Spam Answer and attached Reports has been deleted successfully", MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseAnswerReports(int id)
        {
            Feedback feedback = _answerRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Answer has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report News

        public ViewResult ManageReportedNews(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedNewsViewModel
            {
                News = _newsRepository.GetAllReportedNews(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedNews(int id)
        {
            Feedback feedback = _newsRepository.DeleteReportedNews(id);
            if (feedback.Success)
            {
                SetMessage("Spam News and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseNewsReports(int id)
        {
            Feedback feedback = _newsRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen News has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Tips

        public ViewResult ManageReportedTip(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedTipViewModel
            {
                Tips = _tipRepository.GetAllReportedTips(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedTip(int id)
        {
            Feedback feedback = _tipRepository.DeleteReportedTip(id);
            if (feedback.Success)
            {
                SetMessage("Spam Tip and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseTipReports(int id)
        {
            Feedback feedback = _tipRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Tip has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

        #region Manage Report Recipes

        public ViewResult ManageReportedRecipe(int? minReportNumber)
        {
            if (minReportNumber == null || minReportNumber < 1)
                minReportNumber = AppConfigs.MinReportNumber;
            var vm = new ListReportedRecipeViewModel
            {
                Recipes = _recipeRepository.GetAllReportedRecipes(minReportNumber.Value).Data,
                MinReportNumber = minReportNumber.Value
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult DeleteReportedRecipe(int id)
        {
            Feedback feedback = _recipeRepository.DeleteReportedRecipe(id);
            if (feedback.Success)
            {
                SetMessage("Spam Recipe and attached Reports has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult DeleteFalseRecipeReports(int id)
        {
            Feedback feedback = _recipeRepository.DeleteFalseReports(id);
            if (feedback.Success)
            {
                SetMessage("Reports attached to chosen Recipe has been deleted successfully", MessageType.Success);

            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }


            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        #endregion

    }
}
