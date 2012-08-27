using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface ITipCommentRepository
    {
        Feedback<IList<TipComment>> GetAllReportedTipComments(int minReportNumber);
        Feedback DeleteReportedComment(int id);
        Feedback DeleteFalseReports(int id);
        Feedback<IList<ReportedTipComment>> GetReportedCommentDetails(int id);
        Feedback<IList<TipComment>> GetTipComments(int tipId, PageInfo pageInfo);
        Feedback<IList<TipComment>> GetHighlightTipComments(int tipId, int number);
        Feedback<TipComment> InsertTipComment(TipComment c);
        Feedback DeleteTipComment(TipComment c);
        Feedback<TipComment> VoteTipComment(VotedTipComment vc);
        Feedback ReportTipComment(ReportedTipComment rc);
    }
}