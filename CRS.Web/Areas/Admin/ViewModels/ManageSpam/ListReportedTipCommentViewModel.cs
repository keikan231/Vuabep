using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Business.Models.Entities;
using CRS.Common.DataAnnotations;

namespace CRS.Web.Areas.Admin.ViewModels.ManageSpam
{
    public class ListReportedTipCommentViewModel
    {
        public IList<TipComment> Comments { get; set; }
        public IList<ReportedTipComment> ReportedComments { get; set; }

        [MinExtended(1)]
        [RequiredExtended]
        [IntegerExtended]
        [Display(Name = "Minimum Reports")]
        public int MinReportNumber { get; set; }
    }
}