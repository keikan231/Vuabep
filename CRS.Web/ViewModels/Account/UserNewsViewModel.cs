using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserNewsViewModel
    {
        public IList<News> News { get; set; }
        public int Total { get; set; }
    }
}