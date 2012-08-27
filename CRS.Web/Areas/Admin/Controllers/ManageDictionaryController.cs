using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Resources;
using CRS.Web.Areas.Admin.ViewModels.ManageDictionary;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;
using Telerik.Web.Mvc;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles dictionary management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageDictionary)]
    public class ManageDictionaryController : AdminControllerBase
    {
        //
        // GET: /Admin/ManageDictionary/

        private IWordRepository _repository;
        private IUpdatedWordRepository _updateRepository;

        public ManageDictionaryController(IWordRepository repository, IUpdatedWordRepository updateRepository)
        {
            _repository = repository;
            _updateRepository = updateRepository;
        }

        #region Approve word

        public ActionResult Index()
        {
            ListWordsViewModel vm = new ListWordsViewModel();
            var feedback = _repository.GetAllUnapprovedWords();
            if(feedback.Success)
            {
                vm.Words = feedback.Data;
                return View(vm);
            }
            SetMessage(Messages.GeneralError, MessageType.Error);
            return View();
        }

        public ActionResult Approve(int id)
        {
            var feedback = _repository.ApproveWord(id);

            if (feedback.Success)
            {
                SetMessage(feedback.Message, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("Index");
        }
    
        #endregion

        #region Unapprove word

        public ActionResult ApprovedWordIndex()
        {
            ListWordsViewModel vm = new ListWordsViewModel();
            var feedback = _repository.GetAllApprovedWords();
            if (feedback.Success)
            {
                vm.Words = feedback.Data;
                return View(vm);
            }
            SetMessage(Messages.GeneralError, MessageType.Error);
            return View();
        }

        public ActionResult Unapprove(int id)
        {
            var feedback = _repository.UnapproveWord(id);

            if (feedback.Success)
            {
                SetMessage(feedback.Message, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ApprovedWordIndex");
        }

        #endregion

        #region Delete word

        public ActionResult Delete(int id)
        {
            var feedback = _repository.DeleteWord(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteWordSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteWord(int id)
        {
            var feedback = _repository.DeleteWord(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteWordSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("ApprovedWordIndex");
        }

        #endregion

        #region Approve updated word

        public ActionResult UnApprovedUpdatedWordIndex()
        {
            ListUpdatedWordsViewModel vm = new ListUpdatedWordsViewModel();
            var feedback = _updateRepository.GetAllUnapprovedUpdatedWords();
            if (feedback.Success)
            {
                vm.UpdatedWords = feedback.Data;
                return View(vm);
            }
            SetMessage(Messages.GeneralError, MessageType.Error);
            return View();
        }

        public ActionResult ApproveUpdatedWord(int id)
        {
            var feedback = _updateRepository.ApproveUpdatedWord(id);

            if (feedback.Success)
            {
                SetMessage(feedback.Message, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("UnApprovedUpdatedWordIndex");
        }

        #endregion

        #region Delete updated word
        public ActionResult DeleteUpdatedWord(int id)
        {
            var feedback = _updateRepository.DeleteUpdateWord(id);

            if (feedback.Success)
            {
                SetMessage(Messages.DeleteUpdatedWordSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("UnApprovedUpdatedWordIndex");
        }
        #endregion

    }
}
