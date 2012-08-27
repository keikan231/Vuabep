using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class Question
    {
        #region Primitive properties

        public int Id { get; set; }

        [RequiredExtended]
        [StringLengthExtended(100, MinimumLength = 10)]
        [Display(Name = "Question_Title", ResourceType = typeof(Labels))]
        public string Title { get; set; }

        public string TitleUrl { get; set; }

        [RequiredExtended]
        [Display(Name = "Question_Description", ResourceType = typeof(Labels))]
        [StringLengthExtended(int.MaxValue / 2)] // Don't remove this line otherwise EF will throw a validation exception
        public string ContentText { get; set; }

        public int Views { get; set; }
        [Column("Answers")]
        public int AnswerTimes { get; set; }
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

        [ForeignKey("PostedById")]
        public virtual User PostedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User UpdatedBy { get; set; }

        public virtual ICollection<ReportedQuestion> ReportedQuestions { get; set; }

        [InverseProperty("Question")]
        public virtual ICollection<Answer> Answers { get; set; }

        #endregion
    }
}