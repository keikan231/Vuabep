using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageRoles
{
    public class CheckedPermission
    {
        public bool IsChecked { get; set; }
        public Permission Permission { get; set; }
    }
}