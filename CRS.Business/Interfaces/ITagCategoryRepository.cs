using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface ITagCategoryRepository
    {
        Feedback<IList<TagCategory>> GetAllTagCategories();
        Feedback<TagCategory> InsertTagCategory(TagCategory c);
        Feedback DeleteTagCategory(int id);
        Feedback<TagCategory> GetTagCategoryDetails(int id);
        Feedback<TagCategory> UpdateTagCategory(TagCategory c);
        Feedback<int> IncreaseSearchTimes(int id);
    }
}