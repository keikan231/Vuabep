using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class VisitorNumber
    {
        #region Primitive properties

        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Column("VisitorNumber")]
        public int Visitors { get; set; }

        #endregion
    }
}