using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CaptchaMVC.Attribute;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Resources;
using CRS.Web.Framework.Filters;
using CRS.Web.Framework.Security;
using CRS.Web.Models;
using CRS.Web.Models.FileManagement;
using CRS.Web.ViewModels.Account;

namespace CRS.Web.Controllers
{
    public class AccountController : FrontEndControllerBase
    {
        #region Variables

        private ISecurityRepository _securityRepository;
        private IUserRepository _userRepository;
        private IUploadHandler _uploadHandler;
        private IFileManager _fileManager;

        #endregion

        #region Constructor

        public AccountController(
            ISecurityRepository securityRepository,
            IUserRepository userRepository,
            IUploadHandler uploadHandler,
            IFileManager fileManager)
        {
            _securityRepository = securityRepository;
            _fileManager = fileManager;
            _uploadHandler = uploadHandler;
            _userRepository = userRepository;
        }

        #endregion

        #region Public Methods

        #region Login Methods

        public ActionResult Login(string returnUrl)
        {
            // If already logged in
            if (Request.IsAuthenticated)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Login by website account
        /// </summary>
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            // If already logged in
            if (Request.IsAuthenticated)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var feedback = _securityRepository.Login(model.Username, model.Password);
                if (feedback.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", feedback.Message);
                }
            }

            return View(model);
        }

        #endregion

        #region Logout Methods

        /// <summary>
        /// Logoff and clears forms authentication cookie
        /// </summary>
        [PermissionAuthorize]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            SecurityCacheManager.RemovePrincipal(CurrentUser.UserInfo.Username);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register Methods

        public ViewResult Register()
        {
            // If already logged in
            if (Request.IsAuthenticated)
            {
                object viewmodel = GetPersonalInfo();
                return View("Profile", viewmodel);
            }
            return View();
        }

        [HttpPost]
        [CaptchaVerify("Ký tự nhập không đúng")]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password
                };

                Feedback feedback = _securityRepository.Register(newUser);

                if (feedback.Success)
                {
                    // Send welcome email to user
                    MailController mailController = new MailController();
                    try
                    {
                        mailController.WelcomeEmail(model).Deliver();
                    }
                    catch
                    {
                        SetMessage("Có lỗi xảy ra. Hệ thống không thể gửi mail chúc mừng cho bạn.", MessageType.Error);
                    }
                    return View("WelcomeEmail", model);
                }
                else
                {
                    ModelState.AddModelError("", feedback.Message);
                }
            }

            return View(model);
        }

        #endregion

        #region Reset Password Methods

        public ActionResult RequestResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestResetPassword(RequestResetPasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Insert ResetPassword key into DB
                var tmp = string.Format("{0}{1}", Guid.NewGuid(), Guid.NewGuid());
                var resetPasswordKey = tmp.Replace("-", "");

                var feedback = _securityRepository.RequestResetPassword(vm.Email, resetPasswordKey);
                if (feedback.Success)
                {
                    // Send verification email
                    MailController mailController = new MailController();
                    vm.Username = feedback.Data.Username;
                    vm.ResetPasswordKey = resetPasswordKey;
                    vm.ResetPasswordLink = string.Format("http://{0}/account/resetpassword?key={1}", Request.Url.Authority,
                                                        resetPasswordKey);
                    try
                    {
                        mailController.ResetPasswordVerificationEmail(vm).Deliver();

                        return View("RequestPasswordSuccess");
                    }
                    catch
                    {
                        SetMessage("Có lỗi xảy ra. Hệ thống không thể gửi mail xác nhận quên mật khẩu cho bạn.", MessageType.Error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", feedback.Message);
                }
            }

            return View(vm);
        }

        public ActionResult ResetPassword()
        {
            // Get key in query string
            var resetPasswordKey = Request.QueryString["key"];
            if (!string.IsNullOrEmpty(resetPasswordKey))
            {
                var feeback = _securityRepository.CheckResetPasswordKey(resetPasswordKey);
                if (feeback.Success)
                    return View();
            }

            return View("IncorrectResetPasswordKey");
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Get key in query string
            var resetPasswordKey = Request.QueryString["key"];
            if (!string.IsNullOrEmpty(resetPasswordKey))
            {
                // Get requested email
                var checkFeeback = _securityRepository.SetResetPasswordKey(resetPasswordKey);
                if (!checkFeeback.Success)
                    return View("IncorrectResetPasswordKey");

                var feedback = _securityRepository.ResetPassword(checkFeeback.Data.Email, vm.Password);
                if (feedback.Success)
                {
                    return View("ResetPasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", feedback.Message);
                    return View(vm);
                }
            }
            else
            {
                return View("IncorrectResetPasswordKey");
            }
        }

        #endregion

        #region Update User Profile Methods

        [PermissionAuthorize]
        public ActionResult Profile()
        {
            object viewmodel = GetPersonalInfo();
            return View(viewmodel);
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult Profile(UpdateUserProfileViewModel vm)
        {
            object viewmodel = null;
            if (ModelState.IsValid)
            {
                vm.User.Id = CurrentUser.UserInfo.Id;
                string newUrl = null;

                if (vm.File != null)
                {
                    var uploadFeedback = _uploadHandler.UploadUserAvatar(CurrentUser.UserInfo.Id, vm.File);
                    if (!uploadFeedback.Success)
                    {
                        string msg = string.Format("Cập nhật ảnh đại diện không thành công. Chỉ hỗ trợ ảnh định dạng JPG và PNG. " +
                              "Kích thước ảnh không được vượt quá {0}Mb. Nếu lỗi vẫn xảy ra bạn hãy thử lại vào lúc khác.",
                              AppConfigs.UploadImageMaxSize);
                        SetMessage(msg, MessageType.Error);
                        viewmodel = GetPersonalInfo();
                        return View(viewmodel);
                    }

                    newUrl = string.Format("{0}{1}{2}", uploadFeedback.Data[0], Constants.ImageUrlsSeparator,
                                                  uploadFeedback.Data[1]);
                    vm.User.AvatarUrl = newUrl;
                    CurrentUser.UserInfo.AvatarUrl = newUrl;
                }

                var feedback = _userRepository.UpdateUserProfile(vm.User);

                if (feedback.Success)
                {
                    SetMessage(Messages.UpdateUserProfileSuccess, MessageType.Success);

                    // Delete old avatar files
                    if (!string.IsNullOrEmpty(feedback.Data))
                        _fileManager.DeleteImagePair(feedback.Data);
                    viewmodel = GetPersonalInfo();
                    return View(viewmodel);
                }
                else
                {
                    // If update to DB unsuccessfully, delete new avatar files
                    if (!string.IsNullOrEmpty(newUrl))
                        _fileManager.DeleteImagePair(newUrl);
                }
            }

            SetMessage(Messages.UpdateInformationFail, MessageType.Error);
            viewmodel = GetPersonalInfo();
            return View(viewmodel);
        }

        [PermissionAuthorize]
        public ActionResult GetTabDetails(int id = 0)
        {
            string viewName = string.Empty;
            object viewModel = null;
            switch (id)
            {
                case 1:
                    viewName = "_UserDetailsInformation";
                    viewModel = GetPersonalInfo();
                    break;
                case 2:
                    viewName = "_EditUserInformation";
                    viewModel = GetPersonalInfo();
                    break;
                case 3:
                    viewName = "_UserRecipe";
                    viewModel = GetUserRecipe(CurrentUser.UserInfo.Id);
                    break;
                case 4:
                    viewName = "_UserTip";
                    viewModel = GetUserTip(CurrentUser.UserInfo.Id);
                    break;
                case 5:
                    viewName = "_UserNews";
                    viewModel = GetUserNews(CurrentUser.UserInfo.Id);
                    break;
                case 6:
                    viewName = "_UserQuestion";
                    viewModel = GetUserQuestion(CurrentUser.UserInfo.Id);
                    break;
                case 7:
                    viewName = "_UserAnswer";
                    viewModel = GetUserAnswer(CurrentUser.UserInfo.Id);
                    break;
                case 8:
                    viewName = "_UserFavoriteRecipe";
                    viewModel = GetUserFavoriteRecipe(CurrentUser.UserInfo.Id);
                    break;
                case 9:
                    viewName = "_StatisticInformation";
                    viewModel = GetPersonalInfo();
                    break;
            }

            return PartialView(viewName, viewModel);
        }

        [HttpPost]
        [PermissionAuthorize]
        public JsonResult RemoveUserAvatar()
        {
            var feedback = _userRepository.RemoveAvatar(CurrentUser.UserInfo.Id);

            if (feedback.Success)
            {
                CurrentUser.UserInfo.AvatarUrl = null;

                // Delete old avatar files
                if (!string.IsNullOrEmpty(feedback.Data))
                    _fileManager.DeleteImagePair(feedback.Data);

                // Construct anonymous avatar url to return
                UrlHelper urlHelper = new UrlHelper(Request.RequestContext);
                var path = AppConfigs.AvatarFolderPath + Constants.AnonymousAvatar;

                return Json(new
                {
                    Success = true,
                    AnonymousAvatar = urlHelper.Content(path)
                });
            }

            return Json(feedback);
        }

        #endregion

        #region Change Password Methods

        [PermissionAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult ChangePassword(ChangePasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var feeback = _securityRepository.ChangePassword(vm.OldPassword, vm.NewPassword, CurrentUser.UserInfo.Id);

                if (feeback.Success)
                {
                    SetMessage(Messages.UpdatePasswordSuccess, MessageType.Success);
                    return View(vm);
                }
                else
                {
                    SetMessage(feeback.Message, MessageType.Error);
                    return View(vm);
                }
            }

            SetMessage(Messages.UpdatePasswordFail, MessageType.Error);
            return View(vm);
        }

        [PermissionAuthorize]
        public ActionResult CancelChangePassword()
        {
            return RedirectToAction("Profile", "Account");
        }

        #endregion

        #region View Other User Information Methods

        public ActionResult Details(int id = 0)
        {
            if (CurrentUser != null)
            {
                if (id == CurrentUser.UserInfo.Id)
                    return RedirectToAction("Profile", "Account");
            }

            object vm = GetUserDetailsInfo(id);

            if (vm != null)
            {
                return View(vm);
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetUserTabDetails(int id = 0, int userid = 0)
        {
            string viewName = string.Empty;
            object viewModel = null;
            switch (id)
            {
                case 1:
                    viewName = "_UserDetailsInformation";
                    viewModel = GetUserDetailsInfo(userid);
                    break;
                case 2:
                    viewName = "_StatisticInformation";
                    viewModel = GetUserDetailsInfo(userid);
                    break;
                case 3:
                    viewName = "_UserRecipe";
                    viewModel = GetUserRecipe(userid);
                    break;
                case 4:
                    viewName = "_UserTip";
                    viewModel = GetUserTip(userid);
                    break;
                case 5:
                    viewName = "_UserNews";
                    viewModel = GetUserNews(userid);
                    break;
                case 6:
                    viewName = "_UserQuestion";
                    viewModel = GetUserQuestion(userid);
                    break;
                case 7:
                    viewName = "_UserAnswer";
                    viewModel = GetUserAnswer(userid);
                    break;
            }

            return PartialView(viewName, viewModel);
        }

        #endregion

        #region Unauthorized

        public ActionResult Unauthorized()
        {
            SetMessage(Messages.Unauthorized, MessageType.Error);
            return View();
        }

        #endregion

        #endregion

        #region Private Methods

        //TODO - KIENTT - Personal missing

        private object GetPersonalInfo()
        {
            var feedback = _userRepository.GetPersonalInfo(CurrentUser.UserInfo.Id);

            if (feedback.Success)
            {
                return new UpdateUserProfileViewModel
                {
                    User = feedback.Data,
                    Locations = PrepareLocations()
                };
            }

            return "Có lỗi xảy ra. Lấy thông tin cá nhân không thành công.";
        }

        private object GetUserDetailsInfo(int id)
        {
            var feedback = _userRepository.ViewOtherUserInformaton(id);

            if (feedback.Success)
            {
                return new UpdateUserProfileViewModel
                           {
                               User = feedback.Data
                           };
            }

            SetMessage(feedback.Message, MessageType.Error);
            return null;
        }

        private UserQuestionViewModel GetUserQuestion(int userId)
        {
            var feedback = _userRepository.GetUserQuestion(userId);

            if (feedback.Success)
            {
                return new UserQuestionViewModel
                           {
                               Questions = feedback.Questions,
                               Total = feedback.Total
                           };
            }

            return null;
        }

        private UserAnswerViewModel GetUserAnswer(int userId)
        {
            var feedback = _userRepository.GetUserAnswer(userId);

            if (feedback.Success)
            {
                return new UserAnswerViewModel
                {
                    Answers = feedback.Answers,
                    Total = feedback.Total
                };
            }

            return null;
        }

        private UserTipViewModel GetUserTip(int userId)
        {
            var feedback = _userRepository.GetUserTip(userId);

            if (feedback.Success)
            {
                return new UserTipViewModel
                           {
                               Tips = feedback.Tips,
                               Total = feedback.Total,
                           };
            }

            return null;
        }

        private UserNewsViewModel GetUserNews(int userId)
        {
            var feedback = _userRepository.GetUserNews(userId);

            if (feedback.Success)
            {
                return new UserNewsViewModel
                {
                    News = feedback.News,
                    Total = feedback.Total,
                };
            }

            return null;
        }

        private UserRecipeViewModel GetUserRecipe(int userId)
        {
            var feedback = _userRepository.GetUserRecipe(userId);

            if (feedback.Success)
            {
                return new UserRecipeViewModel
                {
                    Recipes = feedback.Recipes,
                    Total = feedback.Total,
                };
            }

            return null;
        }

        private UserFavoriteRecipeViewModel GetUserFavoriteRecipe(int userId)
        {
            var feedback = _userRepository.GetUserFavoriteRecipes(userId);

            if (feedback.Success)
            {
                return new UserFavoriteRecipeViewModel
                           {
                               FavoriteRecipes = feedback.Recipes,
                               Total = feedback.Total,
                           };
            }

            return null;
        }

        [HttpPost]
        [PermissionAuthorize]
        public ActionResult DeleteUserFavoriteRecipes(List<int> ids)
        {
            var feedback = _userRepository.DeleteUserFavoriteRecipes(ids, CurrentUser.UserInfo.Id);
            if (feedback.Success)
            {
                var viewModel = GetUserFavoriteRecipe(CurrentUser.UserInfo.Id);
                return PartialView("_FavoriteList", viewModel);
            }

            return Content("Error");
        }

        private IList<Location> PrepareLocations()
        {
            var locations = ReferenceDataCache.LocationCollection.ToList();
            return locations;
        }

        #endregion

    }
}
