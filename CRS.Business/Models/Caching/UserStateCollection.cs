using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class UserStateCollection : CacheCollection<UserState>
    {
        private IUserRepository _repository;

        public UserStateCollection(IUserRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<UserState>

        public override void Execute()
        {
            Feedback<IList<UserState>> feedback = _repository.GetAllUserStates();
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