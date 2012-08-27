using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IUpdatedWordRepository
    {
        Feedback<IList<UpdatedWord>> GetAllUnapprovedUpdatedWords();
        Feedback<UpdatedWord> ApproveUpdatedWord(int id);
        Feedback<UpdatedWord> InsertUpdatedWord(UpdatedWord w);
        Feedback DeleteUpdateWord(int id);
    }
}