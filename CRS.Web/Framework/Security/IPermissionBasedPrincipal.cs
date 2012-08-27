using System.Security.Principal;

namespace CRS.Web.Framework.Security
{
    public interface IPermissionBasedPrincipal : IPrincipal
    {
        UserInfo UserInfo { get; set; }
        bool HasPermission(string permission);
    }
}