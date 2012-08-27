using System;
using System.Linq;
using System.Security.Principal;
using CRS.Business.Models.Caching;

namespace CRS.Web.Framework.Security
{
    public class PermissionBasedPrincipal : IPermissionBasedPrincipal
    {
        public PermissionBasedPrincipal(IIdentity identity, UserInfo userInfo)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            if (userInfo == null)
                throw new ArgumentNullException("userInfo");

            Identity = identity;
            UserInfo = userInfo;
        }

        #region Implementation of IPermissionBasedPrincipal

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            return UserInfo.Roles.Any(i => i.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        public UserInfo UserInfo { get; set; }

        public bool HasPermission(string permission)
        {
            return ReferenceDataCache.RoleCollection
                .Any(i => UserInfo.Roles.Contains(i.Name) && i.RolePermissions.Any(j => j.Permission.Name.Equals(permission, StringComparison.OrdinalIgnoreCase)));
        }

        #endregion
    }
}