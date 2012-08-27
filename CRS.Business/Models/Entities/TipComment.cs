﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class TipComment
    {
        #region Primitive properties

        public int Id { get; set; }
        public int TipId { get; set; }

        [RequiredExtended]
        [Display(Name = "Comment_ContentText", ResourceType = typeof(Labels))]
        [StringLengthExtended(4000)]
        public string ContentText { get; set; }

        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int Reports { get; set; }

        [Column("PostedBy")]
        public int PostedById { get; set; }

        public DateTime PostedDate { get; set; }

        [Column("UpdatedBy")]
        public int? UpdatedById { get; set; }

        public DateTime? LastUpdate { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("TipId")]
        public virtual Tip Tip { get; set; }

        [ForeignKey("PostedById")]
        public virtual User PostedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User UpdatedBy { get; set; }

        public virtual ICollection<ReportedTipComment> ReportedComments { get; set; }
        public virtual ICollection<VotedTipComment> VotedComments { get; set; }

        #endregion
    }
}