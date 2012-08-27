using CRS.Business.Feedbacks;
using CRS.Business.Models;

namespace CRS.Business.Interfaces
{
    public interface IAllContentRepository
    {
        SearchAllContentFeedback SearchAllContent(SearchCriteria criteria);
    }
}