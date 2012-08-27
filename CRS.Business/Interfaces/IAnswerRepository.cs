using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IAnswerRepository
    {
        Feedback<IList<Answer>> GetAllReportedAnswers(int minReportNumber);
        Feedback<IList<ReportedAnswer>> GetReportedAnswerDetails(int id);
        Feedback DeleteReportedAnswer(int id);
        Feedback DeleteFalseReports(int id);
        ListAnswerFeedback GetAllAnswers(int id, PageInfo pageInfo);
        Feedback<Answer> InsertAnswer(Answer a);
        Feedback DeleteAnswer(Answer answer);
        Feedback<Answer> VoteAnswer(VotedAnswer va);
        Feedback ReportAnswer(ReportedAnswer ra);
        Feedback<IList<Answer>> GetHighlightAnswers(int id, int number);
    }
}