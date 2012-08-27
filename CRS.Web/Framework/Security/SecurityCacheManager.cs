using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Caching;
using Microsoft.Practices.Unity;

namespace CRS.Web.Framework.Security
{
    /// <summary>
    /// Performs caching operations on security objects
    /// </summary>
    public class SecurityCacheManager
    {
        private static readonly object _lockObject = new object();

        private static string GetPrincipalKey(string username)
        {
            return string.Concat("Principal_", username);
        }

        /// <summary>
        /// Get cached principal. Create new one and insert to cache if not existed
        /// </summary>
        public static PermissionBasedPrincipal GetPrincipal(string username)
        {
            // TODO: consider allowing to call this method for current user only!
            string key = GetPrincipalKey(username);

            // Get cached principal
            object data = Cacher.GetData(key);

            if (data == null) // If not existed create new one and insert to cache
            {
                lock (_lockObject) // Double-checked locking
                {
                    data = Cacher.GetData(key);
                    if (data == null)
                    {
                        ISecurityRepository repository = IoC.UnityContainer.Resolve<ISecurityRepository>();
                        Feedback<User> feedback = repository.GetBasicUserInfo(username);
                        if (feedback.Success)
                        {
                            IList<string> roles = new List<string>();
                            foreach (var userInRole in feedback.Data.UserRoles)
                            {
                                var role = ReferenceDataCache.RoleCollection.FirstOrDefault(i => i.Id == userInRole.RoleId);
                                if (role != null)
                                    roles.Add(role.Name);
                            }
                            UserInfo info = new UserInfo
                            {
                                Id = feedback.Data.Id,
                                Email = feedback.Data.Email,
                                Username = username,
                                AvatarUrl = feedback.Data.AvatarUrl,
                                Point = feedback.Data.Point,
                                Roles = roles
                            };
                            data = new PermissionBasedPrincipal(new PermissionBasedIdentity(username), info);
                            Cacher.AddToCache(key, data, CacheItemPriority.High,
                                CacheHelper.GetAbsoluteExpirationFromConfig(Constants.ConfigKeys.Caching.PrincipalCacheMinutes));
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            return (PermissionBasedPrincipal)data;
        }

        /// <summary>
        /// Remove cached principal
        /// </summary>
        public static void RemovePrincipal(string username)
        {
            Cacher.Remove(GetPrincipalKey(username));
        }
    }
}