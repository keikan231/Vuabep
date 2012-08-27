using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.Words
{
    public class InsertWordViewModel
    {
        public Word Word { get; set; }
        public UpdatedWord UpdateWord { get; set; }
        public string OldValue { get; set; }
    }
}