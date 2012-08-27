using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Common.DataAnnotations;

namespace CRS.Business.Models.Entities
{
    public class Role
    {
        #region Primitive properties

        public int Id { get; set; }

        [RequiredExtended]
        [StringLengthExtended(100)]
        public string Name { get; set; }

        [StringLengthExtended(1000)]
        public string Description { get; set; }

        #endregion

        #region Navigation properties

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        #endregion
    }
}