using CRS.Business.Feedbacks;

namespace CRS.Business.Interfaces
{
    public interface IApplicationRepository
    {
        Feedback<int> IncreasePageView();
        StatisticsFeedback GetStatistics();
    }
}