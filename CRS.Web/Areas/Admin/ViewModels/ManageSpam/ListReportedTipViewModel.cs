using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Business.Models.Entities;
using CRS.Common.DataAnnotations;

namespace CRS.Web.Areas.Admin.ViewModels.ManageSpam
{
    public class ListReportedTipViewModel
    {
        public IList<Tip> Tips;
        public IList<ReportedTip> ReportedTips;

        [MinExtended(1)]
        [RequiredExtended]
        [IntegerExtended]
        [Display(Name = "Minimum Reports")]
        public int MinReportNumber { get; set; }
    }
}