using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageContents
{
    public class ListQuestionsViewModel
    {
        public IList<Question> Questions { get; set; }
    }
}