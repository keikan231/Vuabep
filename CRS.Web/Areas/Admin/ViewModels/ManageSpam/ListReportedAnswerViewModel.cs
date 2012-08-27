using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Business.Models.Entities;
using CRS.Common.DataAnnotations;

namespace CRS.Web.Areas.Admin.ViewModels.ManageSpam
{
    public class ListReportedAnswerViewModel
    {
        public IList<Answer> Answers;
        public IList<ReportedAnswer> ReportedAnswers;

        [MinExtended(1)]
        [RequiredExtended]
        [IntegerExtended]
        [Display(Name = "Minimum Reports")]
        public int MinReportNumber { get; set; }
    }
}