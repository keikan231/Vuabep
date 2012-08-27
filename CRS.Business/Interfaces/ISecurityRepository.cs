using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface ISecurityRepository
    {
        Feedback Login(string email, string password);
        Feedback<User> Register(User user);
        Feedback<User> GetBasicUserInfo(string username);
        Feedback ResetPassword(string email, string newPassword);
        Feedback<User> RequestResetPassword(string email, string resetPasswordKey);
        Feedback<User> CheckResetPasswordKey(string resetPasswordKey);
        Feedback<User> SetResetPasswordKey(string resetPasswordKey);
        Feedback ChangePassword(string oldPassword, string newPassword, int userId);

        /// Performs role operations
        Feedback<IList<Permission>> GetAllPermissions();
        Feedback<IList<Role>> GetAllRoles();
        Feedback<Role> InsertRole(Role role, IList<int> permissionIds);
        Feedback DeleteRole(int roleId);
        Feedback<Role> GetRoleDetails(int roleId);
        Feedback<Role> UpdateRole(Role c, IList<int> permissionIds);
    }
}