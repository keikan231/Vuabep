using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CRS.Web.Framework.Security
{
    public class PermissionBasedIdentity : IIdentity
    {
        public PermissionBasedIdentity(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        #region Implementation of IIdentity

        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        #endregion
    }
}