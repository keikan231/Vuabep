using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Caching;
using CRS.Web.Areas.Admin.ViewModels.ManageRoles;
using CRS.Web.Framework.Filters;
using CRS.Web.Models;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles user management pages
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ManageUsers)]
    public class ManageRolesController : AdminControllerBase
    {
        private ISecurityRepository _repository;

        public ManageRolesController(ISecurityRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            ListRoleViewModel vm = new ListRoleViewModel { Roles = ReferenceDataCache.RoleCollection };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Feedback feedback = _repository.DeleteRole(id);
            if (feedback.Success)
            {
                // Remove from cache
                ReferenceDataCache.RoleCollection.RemoveAll(i => i.Id == id);

                SetMessage(Resources.Messages.DeleteRoleSuccess, MessageType.Success);
            }
            else
            {
                SetMessage(feedback.Message, MessageType.Error);
            }

            // Redirect to current page after deleting
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ViewResult Create()
        {
            var viewModel = new InsertRoleViewModel { Role = new Role(), Permission = new List<CheckedPermission>() };
            foreach (var permission in ReferenceDataCache.PermissionCollection)
            {
                viewModel.Permission.Add(new CheckedPermission { Permission = permission });
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(InsertRoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<int> p = (from t in vm.Permission where t.IsChecked select t.Permission.Id).ToList();

                Feedback<Role> feedback = _repository.InsertRole(vm.Role, p);
                if (feedback.Success)
                {
                    // Remove RoleCollection from cache to force reload on next access
                    Cacher.Remove(Constants.ConfigKeys.Caching.RoleCollectionCacheMinutes);

                    SetMessage(Resources.Messages.InsertRoleSuccess, MessageType.Success);

                    return RedirectToAction("Index");
                }
                SetMessage(feedback.Message, MessageType.Error);
            }

            return View(vm);
        }

        public ActionResult Edit(int id = 0)
        {
            var feedback = _repository.GetRoleDetails(id);

            if (feedback.Success)
            {
                IList<CheckedPermission> checkedPermissions = new List<CheckedPermission>();
                foreach (Permission t in ReferenceDataCache.PermissionCollection)
                {
                    CheckedPermission checkedPermission = new CheckedPermission();
                    checkedPermission.Permission = t;
                    if (feedback.Data.RolePermissions.Any(i => i.PermissionId == t.Id))
                        checkedPermission.IsChecked = true;

                    checkedPermissions.Add(checkedPermission);
                }
                return View(new InsertRoleViewModel { Role = feedback.Data, Permission = checkedPermissions });
            }

            SetMessage(feedback.Message, MessageType.Error);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(InsertRoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<int> p = (from t in vm.Permission where t.IsChecked select t.Permission.Id).ToList();

                Feedback<Role> feedback = _repository.UpdateRole(vm.Role, p);
                if (feedback.Success)
                {
                    // Remove RoleCollection from cache to force reload on next access
                    Cacher.Remove(Constants.ConfigKeys.Caching.RoleCollectionCacheMinutes);

                    SetMessage(Resources.Messages.UpdateRoleSuccess, MessageType.Success);

                    return RedirectToAction("Index");
                }
                SetMessage(feedback.Message, MessageType.Error);
            }
            return View(vm);
        }
    }
}
