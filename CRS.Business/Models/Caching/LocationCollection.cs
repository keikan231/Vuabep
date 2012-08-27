using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class LocationCollection : CacheCollection<Location>
    {
        private IReferenceDataRepository _repository;

        public LocationCollection(IReferenceDataRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<Location>

        public override void Execute()
        {
            Feedback<IList<Location>> feedback = _repository.GetAllLocations();
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