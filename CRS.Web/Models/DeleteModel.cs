namespace CRS.Web.Models
{
    public class DeleteModel
    {
        public int Id { get; set; }
        public int PostedById { get; set; }
        public string Title { get; set; }
        public string Entity { get; set; }

        public DeleteModel(int id, int postedById, string title, string entity)
        {
            Id = id;
            PostedById = postedById;
            Title = title;
            Entity = entity;
        }
    }
}