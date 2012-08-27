using System.Globalization;
using System.Threading;
using CRS.Business.Models;
using CRS.Web.Controllers;
using CRS.Web.Framework.Filters;

namespace CRS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Base class for all controllers in the Admin area
    /// </summary>
    [PermissionAuthorize(Permissions = KeyObject.Permission.ViewAdminPages)] // At least the user has to login in order to access Admin pages
    public abstract class AdminControllerBase : CrsControllerBase
    {
        protected AdminControllerBase()
        {
            // Back-end pages are displayed with English
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        }
    }
}
