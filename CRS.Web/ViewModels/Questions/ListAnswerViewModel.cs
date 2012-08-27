using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Questions
{
    public class ListAnswerViewModel
    {
        public IList<Answer> Answers { get; set; }
        public bool HasMore { get; set; }
        public int CurrentUserId { get; set; }
    }
}