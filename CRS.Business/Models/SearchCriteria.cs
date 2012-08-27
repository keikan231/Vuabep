namespace CRS.Business.Models
{
    public class SearchCriteria
    {
        public string TitleSearch { get; set; }
        public PageInfo PageInfo { get; set; }
        public Order OrderBy { get; set; }
    }
}