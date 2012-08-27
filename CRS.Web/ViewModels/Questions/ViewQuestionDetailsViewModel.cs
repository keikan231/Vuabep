using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Questions
{
    public class ViewQuestionDetailsViewModel
    {
        public Question Question { get; set; }
        public IList<Answer> Answers { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public bool CanEdit { get; set; }
        public IList<Answer> HighlightAnswers { get; set; }
    }
}