using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageUsers
{
    public class EditUserViewModel
    {
        public User User { get; set; }

        public IList<CheckedRole> Roles { get; set; }

        public IList<UserState> UserStates { get; set; }
    }
}