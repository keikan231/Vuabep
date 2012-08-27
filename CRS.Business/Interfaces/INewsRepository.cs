using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface INewsRepository
    {
        //News
        Feedback<IList<News>> GetAllReportedNews(int minReportNumber);
        Feedback<IList<ReportedNews>> GetReportedNewsDetails(int id);
        Feedback DeleteReportedNews(int id);
        Feedback DeleteFalseReports(int id);
        ListNewsFeedback GetAllNews(SortCriteria criteria);
        InsertNewsFeedback InsertNews(News q);
        Feedback DeleteNews(int id);
        Feedback<News> GetNewsForEditing(int id);
        Feedback<News> GetNewsDetails(int id);
        Feedback<News> UpdateNews(News q);
        Feedback ReportNews(ReportedNews rq);
        Feedback<IList<News>> GetTopNews(int take);
        SearchNewsFeedback SearchNews(SearchCriteria criteria);
        SearchNewsFeedback SortNews(SortCriteria criteria, int take);
    }
}