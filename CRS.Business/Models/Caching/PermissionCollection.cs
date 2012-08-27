using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class PermissionCollection : CacheCollection<Permission>
    {
        private ISecurityRepository _repository;

        public PermissionCollection(ISecurityRepository repository)
        {
            _repository = repository;
        }

        public override void Execute()
        {
            Feedback<IList<Permission>> feedback = _repository.GetAllPermissions();
            IsPopulated = feedback.Success;
            if (feedback.Success)
            {
                Clear();
                AddRange(feedback.Data);
            }
        }
    }
}