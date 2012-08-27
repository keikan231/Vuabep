using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.TipComments
{
    public class ListTipCommentViewModel
    {
        public int TipId { get; set; }
        public IList<TipComment> Comments { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public IList<TipComment> HighlightComments { get; set; }
    }
}