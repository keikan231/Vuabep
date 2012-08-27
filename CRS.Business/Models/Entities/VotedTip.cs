using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class VotedTip
    {
        #region Primitive properties

        public int Id { get; set; }
        public int TipId { get; set; }
        [Column("VotedBy")]
        public int VotedById { get; set; }
        public DateTime VotedDate { get; set; }
        public bool IsUpVote { get; set; }

        #endregion

        #region Navigation properties

        public virtual Tip Tip { get; set; }
        [ForeignKey("VotedById")]
        public virtual User VotedBy { get; set; }

        #endregion
    }
}