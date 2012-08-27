using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Words
{
    public class ListWordsViewModel
    {
        public IList<Word> Words { get; set; }
    }
}