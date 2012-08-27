using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface ITipCategoryRepository
    {
        Feedback<IList<TipCategory>> GetAllTipCategories();
        Feedback<TipCategory> InsertTipCategory(TipCategory tipCategory);
        Feedback<TipCategory> GetTipCategoryDetails(int id);
        Feedback<TipCategory> UpdateTipCategory(TipCategory tipCategory);
        Feedback DeleteTipCategory(int id);
    }
}