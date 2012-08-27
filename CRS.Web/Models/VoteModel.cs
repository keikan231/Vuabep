namespace CRS.Web.Models
{
    public class VoteModel
    {
        public int Id { get; set; }
        public int Votes { get; set; }
        public string Title { get; set; }
        public string Entity { get; set; }
        public bool CanVote { get; set; }


        public VoteModel(int id, string title, int votes, string entity, bool canVote = false)
        {
            Id = id;
            Title = title;
            Votes = votes;
            Entity = entity;
            CanVote = canVote;
        }
    }
}