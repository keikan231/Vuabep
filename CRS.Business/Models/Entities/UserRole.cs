namespace CRS.Business.Models.Entities
{
    public class UserRole
    {
        #region Primitive properties

        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        #endregion

        #region Navigation properties

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }

        #endregion
    }
}