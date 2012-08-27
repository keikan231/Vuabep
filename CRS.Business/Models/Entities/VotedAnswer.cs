using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class VotedAnswer
    {
        #region Primitive properties

        public int Id { get; set; }
        public int AnswerId { get; set; }
        [Column("VotedBy")]
        public int VotedById { get; set; }
        public DateTime VotedDate { get; set; }
        public bool IsUpVote { get; set; }

        #endregion

        #region Navigation properties

        public virtual Answer Answer { get; set; }
        [ForeignKey("VotedById")]
        public virtual User VotedBy { get; set; }

        #endregion
    }
}