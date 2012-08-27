using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Account
{
    public class UserAnswerViewModel
    {
        public IList<Answer> Answers { get; set; }
        public int Total { get; set; }
    }
}