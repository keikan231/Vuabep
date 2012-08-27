using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface INewsCommentRepository
    {
        Feedback<IList<NewsComment>> GetAllReportedNewsComments(int minReportNumber);
        Feedback DeleteReportedComment(int id);
        Feedback DeleteFalseReports(int id);
        Feedback<IList<ReportedNewsComment>> GetReportedCommentDetails(int id);
        Feedback<IList<NewsComment>> GetNewsComments(int newsId, PageInfo pageInfo);
        Feedback<IList<NewsComment>> GetHighlightNewsComments(int newsId, int number);
        Feedback<NewsComment> InsertNewsComment(NewsComment c);
        Feedback DeleteNewsComment(NewsComment c);
        Feedback<NewsComment> VoteNewsComment(VotedNewsComment vc);
        Feedback ReportNewsComment(ReportedNewsComment rc);
    }
}