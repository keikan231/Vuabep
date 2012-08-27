using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageDictionary
{
    public class ListWordsViewModel
    {
        public IList<Word> Words { get; set; }
    }
}