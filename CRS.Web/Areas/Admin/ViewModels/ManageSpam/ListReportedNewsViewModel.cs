using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Business.Models.Entities;
using CRS.Common.DataAnnotations;

namespace CRS.Web.Areas.Admin.ViewModels.ManageSpam
{
    public class ListReportedNewsViewModel
    {
        public IList<News> News;
        public IList<ReportedNews> ReportedNews;

        [MinExtended(1)]
        [RequiredExtended]
        [IntegerExtended]
        [Display(Name = "Minimum Reports")]
        public int MinReportNumber { get; set; }
    }
}