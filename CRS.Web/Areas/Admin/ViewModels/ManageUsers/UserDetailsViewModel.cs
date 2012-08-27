using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageUsers
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public string RolesSummary { get; set; }
    } 
}