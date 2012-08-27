namespace CRS.Web.Models.Searching
{
    public class SearchInput
    {
        public string Name { get; set; }
        public string NameUrl { get; set; }
        public string CategoryName { get; set; }
        public int? Page { get; set; }
        public string Sort { get; set; }
    }
}