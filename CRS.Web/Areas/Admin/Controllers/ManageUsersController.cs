using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Web.Areas.Admin.ViewModels.ManageUsers;
using CRS.Web.Framework.Filters;
using CRS.Web.Framework.Security;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles user management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageUsers)]
    public class ManageUsersController : AdminControllerBase
    {
        private IUserRepository _repository;

        public ManageUsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index(string usernameSearch)
        {
            ListUserViewModel vm = new ListUserViewModel();

            var feedback = _repository.GetSimilarUsers(usernameSearch);

            if (feedback.Success)
            {
                vm.UsernameSearch = usernameSearch;
                vm.Users = feedback.Data;
                foreach (var user in vm.Users)
                {
                    user.UserState = ReferenceDataCache.UserStateCollection.FirstOrDefault(i => i.Id == user.UserStateId);
                }
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var feedback = _repository.DeleteUser(id);
            if (feedback.Success)
            {
                SetMessage(Resources.Messages.DeleteUserSuccess, MessageType.Success);
                // Remove from cache to affect immediately
                SecurityCacheManager.RemovePrincipal(feedback.Data.Email);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id = 0)
        {
            var feedback = _repository.GetUserDetails(id);

            if (feedback.Success)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Role t in ReferenceDataCache.RoleCollection)
                {
                    if (feedback.Data.UserRoles.Any(i => i.RoleId == t.Id))
                        sb.Append(t.Name).Append(", ");
                }

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 2, 2);

                return View(new UserDetailsViewModel
                {
                    User = feedback.Data,
                    RolesSummary = sb.ToString()
                });
            }

            SetMessage(feedback.Message, MessageType.Error);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id = 0)
        {
            var feedback = _repository.GetUserDetails(id);

            if (feedback.Success)
            {
                IList<CheckedRole> checkedRoles = new List<CheckedRole>();
                foreach (Role t in ReferenceDataCache.RoleCollection)
                {
                    CheckedRole checkedRole = new CheckedRole();
                    checkedRole.Role = t;
                    if (feedback.Data.UserRoles.Any(i => i.RoleId == t.Id))
                    {
                        checkedRole.IsChecked = true;
                    }
                    checkedRoles.Add(checkedRole);
                }
                return View(new EditUserViewModel
                {
                    User = feedback.Data,
                    Roles = checkedRoles,
                    UserStates = ReferenceDataCache.UserStateCollection
                });
            }

            SetMessage(feedback.Message, MessageType.Error);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(EditUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<int> p = (from t in vm.Roles where t.IsChecked select t.Role.Id).ToList();

                Feedback<User> feedback = _repository.UpdateUser(vm.User, p);
                if (feedback.Success)
                {
                    SetMessage(Resources.Messages.UpdateUserSuccess, MessageType.Success);
                    // Remove from cache to affect immediately
                    SecurityCacheManager.RemovePrincipal(feedback.Data.Email);

                    return RedirectToAction("Index");
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            vm.UserStates = ReferenceDataCache.UserStateCollection;
            return View(vm);
        }
    }
}
