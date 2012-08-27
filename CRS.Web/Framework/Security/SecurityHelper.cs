using System.Web;
using CRS.Business.Models;
using CRS.Business.Models.Caching;

namespace CRS.Web.Framework.Security
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Gets current PermissionBasedPrincipal
        /// </summary>
        public static IPermissionBasedPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as IPermissionBasedPrincipal;} 
        }

        //TODO - KIENTT - Check later
        public static bool CanEditContentsDirectly()
        {
            return CurrentUser.HasPermission(KeyObject.Permission.EditContents);
        }

        //public static bool CanUploadPlaceImageDirectly()
        //{
        //    return CurrentUser.UserInfo.Point >= ReferenceDataCache.PointConfigCollection.UploadPlaceImageDirectly
        //           || CurrentUser.HasPermission(KeyObject.Permission.UploadPlaceImages);
        //}
    }
}