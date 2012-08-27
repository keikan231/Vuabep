using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Questions
{
    public class ListQuestionViewModel
    {
        public IList<Question> Questions { get; set; }
        public int Page { get; set; }
        public bool HasMore { get; set; }
    }
}