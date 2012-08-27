using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class VotedTipComment
    {
        #region Primitive properties

        public int Id { get; set; }
        public int CommentId { get; set; }
        [Column("VotedBy")]
        public int VotedById { get; set; }
        public DateTime VotedDate { get; set; }
        public bool IsUpVote { get; set; }

        #endregion

        #region Navigation properties

        public virtual TipComment Comment { get; set; }
        [ForeignKey("VotedById")]
        public virtual User VotedBy { get; set; }

        #endregion
    }
}