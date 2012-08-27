using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
        public class TipCategoryCollection : CacheCollection<TipCategory>
        {
            private readonly ITipCategoryRepository _repository;

            public TipCategoryCollection(ITipCategoryRepository repository)
            {
                _repository = repository;
            }

            public override void Execute()
            {
                Feedback<IList<TipCategory>> feedback = _repository.GetAllTipCategories();
                IsPopulated = feedback.Success;
                if (feedback.Success)
                {
                    Clear();
                    AddRange(feedback.Data);
                }
        }
    }
}