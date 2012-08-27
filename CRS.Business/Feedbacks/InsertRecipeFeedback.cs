using CRS.Business.Models.Entities;

namespace CRS.Business.Feedbacks
{
    public class InsertRecipeFeedback : Feedback
    {
        public InsertRecipeFeedback(bool success, string message)
            : base(success, message)
        {
        }

        public int NewPoint { get; set; }
        public Recipe Recipe { get; set; }
    }
}