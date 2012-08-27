using System.Collections.Generic;

namespace CRS.Web.Framework.Security
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public int Point { get; set; }
        public string AvatarUrl { get; set; }
        public IList<string> Roles { get; set; }
    }
}