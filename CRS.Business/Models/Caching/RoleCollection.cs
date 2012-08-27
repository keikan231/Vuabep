using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class RoleCollection : CacheCollection<Role>
    {
        private ISecurityRepository _repository;

        public RoleCollection(ISecurityRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<Role>

        public override void Execute()
        {
            Feedback<IList<Role>> feedback = _repository.GetAllRoles();
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