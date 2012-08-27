using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageUsers
{
    public class CheckedRole
    {
        public bool IsChecked { get; set; }
        public Role Role { get; set; }
    }
}