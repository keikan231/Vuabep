using System.Web.Mvc;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;
using CRS.Web.ViewModels.Words;

namespace CRS.Web.Controllers
{
    public class DictionaryController : FrontEndControllerBase
    {
        private IWordRepository _repository;
        private IUpdatedWordRepository _updateRepository;
        public DictionaryController(IWordRepository repository, IUpdatedWordRepository updateRepository)
        {
            _repository = repository;
            _updateRepository = updateRepository;
        }

        public ActionResult Index()
        {
            ListWordsViewModel words = new ListWordsViewModel();
            var feedback = _repository.GetAllApprovedWords();
            if (feedback.Success)
            {
                words.Words = feedback.Data;
                return View(words);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)] // 10'
        public ActionResult IngredientDictionary()
        {
            ListWordsViewModel words = new ListWordsViewModel();
            var feedback = _repository.GetAllApprovedWords();
            if (feedback.Success)
            {
                words.Words = feedback.Data;
                return View(words);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View();
        }

        public ViewResult Create()
        {
            InsertWordViewModel vm = new InsertWordViewModel() { Word = new Word() };
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Create(InsertWordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Word.CreatedById = CurrentUser.UserInfo.Id;
                var feedback = _repository.InsertWord(vm.Word);
                if (feedback.Success)
                {
                    SetMessage(feedback.Message, MessageType.Success);
                    return RedirectToAction("Index");
                }
                SetMessage(feedback.Message, MessageType.Error);
            }
            return View(vm);
        }

        public ViewResult Update(int id)
        {
            InsertWordViewModel vm = null;
            var feedback = _repository.GetWordForEditing(id);
            if (feedback.Success)
            {
                vm = new InsertWordViewModel
                {
                    Word = feedback.Data,
                    OldValue = feedback.Data.Value
                };
                return View(vm);
            }
            SetMessage(feedback.Message, MessageType.Error);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Update(InsertWordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Word.Value.Trim().Equals(vm.OldValue))
                {
                    SetMessage("Nghĩa mới phải khác nghĩa cũ", MessageType.Error);
                    return View(vm);
                }
                vm.UpdateWord = new UpdatedWord();
                vm.UpdateWord.WordId = vm.Word.Id;
                vm.UpdateWord.NewValue = vm.Word.Value;
                vm.UpdateWord.UpdatedById = CurrentUser.UserInfo.Id;
                vm.UpdateWord.Words = new Word {Id = vm.Word.Id, Key = vm.Word.Key};
                vm.UpdateWord.UpdatedBy = new User { Id = CurrentUser.UserInfo.Id , Username = CurrentUser.UserInfo.Username};
                var feedback = _updateRepository.InsertUpdatedWord(vm.UpdateWord);

                if (feedback.Success)
                {
                    SetMessage(feedback.Message, MessageType.Success);
                    return RedirectToAction("Index");
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

    }
}
