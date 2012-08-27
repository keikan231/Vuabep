using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserTipViewModel
    {
        public IList<Tip> Tips { get; set; }
        public int Total { get; set; }
    }
}