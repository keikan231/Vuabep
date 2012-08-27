using System.Web.Mvc;
using System.Web.UI;
using CRS.Web.Framework.Security;
using CRS.Web.Models;

namespace CRS.Web.Controllers
{
    /// <summary>
    /// Base class for all controllers in the system
    /// </summary>
    [OutputCache(Location = OutputCacheLocation.None)] // Set Cache location to None to avoid IE caching problem
    public abstract class CrsControllerBase : Controller
    {
        /// <summary>
        /// Gets current PermissionBasedPrincipal
        /// </summary>
        public IPermissionBasedPrincipal CurrentUser
        {
            get { return User as IPermissionBasedPrincipal; }
        }

        protected void SetMessage(string message, MessageType messageType)
        {
            TempData["Message"] = message;
            TempData["MessageType"] = messageType;
        }
    }
}
