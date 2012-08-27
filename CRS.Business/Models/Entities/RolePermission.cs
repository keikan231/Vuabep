namespace CRS.Business.Models.Entities
{
    public class RolePermission
    {
        #region Primitive properties

        public int Id { get; set; }
        public int PermissionId { get; set; }
        public int RoleId { get; set; }

        #endregion

        #region Navigation properties

        public virtual Permission Permission { get; set; }
        public virtual Role Role { get; set; }

        #endregion
    }
}