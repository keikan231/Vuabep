using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Business.Models.Entities;
using CRS.Common.DataAnnotations;

namespace CRS.Web.Areas.Admin.ViewModels.ManageUsers
{
    public class ListUserViewModel
    {
        public IList<User> Users { get; set; }
        [RequiredExtended]
        [Display(Name = "Search Username")]
        public string UsernameSearch { get; set; }
    }
}