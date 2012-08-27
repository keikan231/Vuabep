using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CRS.Common.Helpers;
using CRS.Web.Framework.Security;

namespace CRS.Web.Framework.Filters
{
    /// <summary>
    /// AuthorizeAttribute using role and permission. The user is authorized if username is included in Users list, or role is included in Roles list,
    /// or permission is included in Permissions list. Note about the OR conditions instead of AND condition as the base AuthorizeAttribute's behavior.
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            PermissionBasedPrincipal principal = httpContext.User as PermissionBasedPrincipal;
            if (principal == null || principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
                return false;

            // If Users, Roles and Permissions lists are not provided, the user is authenticated if he's logged in
            if (string.IsNullOrEmpty(Users) && string.IsNullOrEmpty(Roles) && string.IsNullOrEmpty(Permissions))
                return true;

            // Check if the current user's name is included in Users list
            if (Users != null)
            {
                string[] users = Users.SplitAndTrim(',');
                if (users.Contains(principal.Identity.Name))
                    return true;
            }

            // Check if the current user's role is included in Roles list
            if (Roles != null)
            {
                string[] roles = Roles.SplitAndTrim(',');
                if (roles.Any(principal.IsInRole))
                    return true;
            }

            // Check if the current user has at least 1 permission included in Permissions lists
            if (Permissions != null)
            {
                string[] permissions = Permissions.SplitAndTrim(',');
                if (permissions.Any(principal.HasPermission))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new JsonResult
                                     {
                                         Data = new { Success = false, Message = "NotAuthorized" },
                                         JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                     };
            }
            else
            {
                // If the user has already logged in but still doesn't have permission to access the requested page
                // redirect to unauthorized page
                if (context.HttpContext.Request.IsAuthenticated)
                {
                    var unauthorizedPage = new { area = (string)null, controller = "Account", action = "Unauthorized" };
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(unauthorizedPage));
                }
                else
                {
                    base.HandleUnauthorizedRequest(context);
                }
            }
        }
    }
}
