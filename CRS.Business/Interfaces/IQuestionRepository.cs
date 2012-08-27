using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IQuestionRepository
    {
        Feedback<IList<Question>> GetAllReportedQuestion(int minReportNumber);
        Feedback<IList<ReportedQuestion>> GetReportedQuestionDetails(int id);
        Feedback DeleteReportedQuestion(int id);
        Feedback DeleteFalseReports(int id);
        ListQuestionFeedback GetAllQuestions(PageInfo pageInfo);
        Feedback<Question> InsertQuestion(Question q);
        Feedback DeleteQuestion(int id);
        Feedback<Question> GetQuestionForEditing(int id);
        Feedback<Question> GetQuestionDetails(int id);
        Feedback<Question> UpdateQuestion(Question q);
        Feedback ReportQuestion(ReportedQuestion rq);
        Feedback<IList<Question>> GetHotQuestionsInMonth(int take);
        Feedback<IList<User>> GetTopAnswerContributors(int take);
        Feedback<IList<Question>> GetNewestQuestions(int take);
    }
}