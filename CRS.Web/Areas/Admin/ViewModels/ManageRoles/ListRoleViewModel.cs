using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageRoles
{
    public class ListRoleViewModel
    {
        public IList<Role> Roles { get; set; }
    }
}