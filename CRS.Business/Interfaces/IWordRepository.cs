using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IWordRepository
    {
        Feedback<IList<Word>> GetAllApprovedWords();
        Feedback<IList<Word>> GetAllUnapprovedWords();
        Feedback<Word> ApproveWord(int id);
        Feedback<Word> InsertWord(Word w);
        Feedback<Word> GetWordForEditing(int id);
        Feedback<Word> GetApprovedWordDetails(int id);
        Feedback DeleteWord(int id);
        Feedback<Word> UnapproveWord(int id);
    }
}