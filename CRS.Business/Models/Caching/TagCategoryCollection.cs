using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
        public class TagCategoryCollection : CacheCollection<TagCategory>
        {
            private ITagCategoryRepository _repository;

            public TagCategoryCollection(ITagCategoryRepository repository)
            {
                _repository = repository;
            }

            #region Overrides of CacheCollection<Category>

            public override void Execute()
            {
                var feedback = _repository.GetAllTagCategories();
                IsPopulated = feedback.Success;
                if (feedback.Success)
                {
                    Clear();
                    AddRange(feedback.Data);
                }
            }

            #endregion
    }
}