using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageRoles
{
    public class InsertRoleViewModel
    {
        public Role Role { get; set; }

        public IList<CheckedPermission> Permission { get; set; }
    }
}