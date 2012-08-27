using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface ITipRepository
    {
        Feedback<IList<Tip>> GetAllReportedTips(int minReportNumber);
        Feedback<IList<ReportedTip>> GetReportedTipDetails(int id);
        Feedback DeleteReportedTip(int id);
        Feedback DeleteFalseReports(int id);
        ListTipFeedback GetAllTip(SortCriteria criteria);
        ListTipFeedback GetTipByCatogory(int tipCategoryId, SortCriteria criteria);
        InsertTipFeedback InsertTip(Tip q);
        Feedback DeleteTip(int id);
        Feedback<Tip> GetTipForEditing(int id);
        Feedback<Tip> GetTipDetails(int id);
        Feedback<Tip> UpdateTip(Tip q);
        Feedback ReportTip(ReportedTip rt);
        Feedback<Tip> VoteTip(VotedTip vt);
        Feedback<IList<Tip>> GetTopTips(int take);
        Feedback<IList<Tip>> GetTopTipsByCategory(int take, int id);
        SearchTipFeedback SearchTips(SearchCriteria criteria);
        SearchTipFeedback SortTips(SortCriteria criteria, int take);
    }
}