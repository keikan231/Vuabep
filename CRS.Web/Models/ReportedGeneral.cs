using System;

namespace CRS.Web.Models
{
    public class ReportedGeneral
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public String ReportedBy { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Reason { get; set; }
    }
}