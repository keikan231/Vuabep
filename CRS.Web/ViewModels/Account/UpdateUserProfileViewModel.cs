using System.Collections.Generic;
using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UpdateUserProfileViewModel
    {
        public User User { get; set; }

        public HttpPostedFileBase File { get; set; }

        public IList<Location> Locations { get; set; }
    }
}