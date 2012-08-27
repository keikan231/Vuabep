using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserQuestionViewModel
    {
        public IList<Question> Questions { get; set; }
        public int Total { get; set; }
    }
}