using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageDictionary
{
    public class ListUpdatedWordsViewModel
    {
        public IList<UpdatedWord> UpdatedWords { get; set; }
    }
}