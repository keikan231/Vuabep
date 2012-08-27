namespace CRS.Web.Models
{
    public class ReportModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Entity { get; set; }

        public ReportModel(int id, string title, string entity)
        {
            Id = id;
            Title = title;
            Entity = entity;
        }
    }
}