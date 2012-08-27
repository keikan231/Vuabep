using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models;
using CRS.Business.Models.Caching;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Cryptography;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        #region Implementation of ISecurityRepository: Login - Logout - Reset password

        public Feedback Login(string username, string password)
        {
            // Password should not be null up to here. But check again for safety reason.
            if (string.IsNullOrEmpty(password))
                return new Feedback(false, Messages.IncorrectLoginInfo);

            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.SingleOrDefault(u => u.Username == username && !u.IsDeleted);
                    // Incorrect email
                    if (user == null)
                        return new Feedback(false, Messages.IncorrectLoginInfo);

                    // Check if exceeded login attempts allowed
                    if (user.FailedLoginAttempts >= AppConfigs.LoginAttemptsAllowed && user.LastLoginAttempted != null
                        && user.LastLoginAttempted.Value.AddMinutes(AppConfigs.LoginAttemptLockMinutes) >= DateTime.Now)
                        return new Feedback(false, string.Format(Messages.AccountIsLocked, AppConfigs.LoginAttemptLockMinutes));

                    // Incorrect password
                    if (user.Password != PasswordCryptography.HashPassword(password))
                    {
                        // Increase login attempts
                        if (user.LastLoginAttempted == null
                            || user.LastLoginAttempted.Value.AddMinutes(AppConfigs.LoginAttemptLockMinutes) < DateTime.Now)
                            user.FailedLoginAttempts = 1;
                        else
                            user.FailedLoginAttempts++;
                        user.LastLoginAttempted = DateTime.Now;

                        entities.SaveChanges();

                        return new Feedback(false, Messages.IncorrectLoginInfo);
                    }

                    // Banned
                    if (user.UserStateId == KeyObject.UserState_Banned)
                        return new Feedback(false, Messages.AccountIsBanned);

                    user.LastLogin = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    user.LastLoginAttempted = null;
                    if (!string.IsNullOrEmpty(user.ResetPasswordKey))
                        user.ResetPasswordKey = null;

                    entities.SaveChanges();

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.LoginFail);
            }
        }       

        public Feedback<User> Register(User user)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check existing email address
                    User emailExisted = entities.Users.FirstOrDefault(i => i.Email == user.Email && !i.IsDeleted);
                    User usernameExisted = entities.Users.FirstOrDefault(i => i.Username == user.Username && !i.IsDeleted);
                    if (emailExisted != null)
                    {
                        return new Feedback<User>(false, Messages.EmailIsUsed, emailExisted);
                    }
                    else if(usernameExisted != null)
                    {
                        return new Feedback<User>(false, Messages.UsernameIsUsed, usernameExisted);
                    }
                    else
                    {
                        User newUser = new User
                        {
                            Username = user.Username,
                            Email = user.Email,
                            Level = KeyObject.Title.PhuBepLevel,
                            Password = PasswordCryptography.HashPassword(user.Password),
                            Point = ReferenceDataCache.PointConfigCollection.StartingPoint,
                            TodayPoint = ReferenceDataCache.PointConfigCollection.StartingPoint,
                            UserStateId = KeyObject.UserState_Active,
                            FailedLoginAttempts = 0,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        entities.Users.Add(newUser);
                        entities.SaveChanges();

                        return new Feedback<User>(true, null, newUser);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false, Messages.GeneralError);
            }
        }

        public Feedback ResetPassword(string email, string newPassword)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    User user = entities.Users.Single(i => i.Email.Equals(email));

                    user.Password = PasswordCryptography.HashPassword(newPassword);
                    user.LastUpdate = DateTime.Now;

                    entities.SaveChanges();

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false);
            }
        }

        public Feedback<User> GetBasicUserInfo(string username)
        {
            using (var entities = new CrsEntities())
            {
                try
                {
                    var user = entities.Users
                        .Include("UserRoles")
                        .Single(u => u.Username == username && !u.IsDeleted && u.UserStateId == KeyObject.UserState_Active);

                    // Treat as a login
                    user.LastLogin = DateTime.Now;
                    if (!string.IsNullOrEmpty(user.ResetPasswordKey))
                        user.ResetPasswordKey = null;
                    entities.SaveChanges();

                    return new Feedback<User>(true, null, user);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new Feedback<User>(false);
                }
            }
        }

        public Feedback<User> RequestResetPassword(string email, string resetPasswordKey)
        {
            using (var entites = new CrsEntities())
            {
                try
                {
                    User exist = entites.Users.SingleOrDefault(i => i.Email.Equals(email) && !i.IsDeleted);
                    if (exist == null)
                    {
                        return new Feedback<User>(false, Messages.EmailIsNotExisted);
                    }
                    else
                    {
                        exist.ResetPasswordKey = resetPasswordKey;
                        exist.LastUpdate = DateTime.Now;

                        entites.SaveChanges();

                        return new Feedback<User>(true, null, exist);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new Feedback<User>(false);
                }
            }
        }

        public Feedback<User> CheckResetPasswordKey(string resetPasswordKey)
        {
            try
            {
                using (var entitties = new CrsEntities())
                {
                    User exist = entitties.Users
                        .Single(i => i.ResetPasswordKey.Equals(resetPasswordKey) && !i.IsDeleted);

                    return new Feedback<User>(true, null, exist);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false);
            }
        }

        public Feedback<User> SetResetPasswordKey(string resetPasswordKey)
        {
            try
            {
                using (var entitties = new CrsEntities())
                {
                    User exist = entitties.Users
                        .Single(i => i.ResetPasswordKey.Equals(resetPasswordKey) && !i.IsDeleted);

                    exist.ResetPasswordKey = null;

                    entitties.SaveChanges();

                    return new Feedback<User>(true, null, exist);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<User>(false);
            }
        }

        public Feedback ChangePassword(string oldPassword, string newPassword, int userId)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var hashOldPassword = PasswordCryptography.HashPassword(oldPassword);

                    User exist = entities.Users
                        .SingleOrDefault(i => i.Id == userId && i.Password.Equals(hashOldPassword));

                    // Old password is not correct
                    if (exist == null)
                    {
                        return new Feedback(false, Messages.IncorrectOldPassword);
                    }
                    else
                    {
                        exist.Password = PasswordCryptography.HashPassword(newPassword);
                        exist.LastUpdate = DateTime.Now;

                        entities.SaveChanges();

                        return new Feedback(true);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.UpdateInformationFail);
            }
        }

        #endregion

        #region Implementation of ISecurityRepository: Admin - Role

        //Performs role operations
        public Feedback<IList<Permission>> GetAllPermissions()
        {
            using (var entities = new CrsEntities())
            {
                try
                {
                    return new Feedback<IList<Permission>>(true, null, entities.Permissions.ToList());
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new Feedback<IList<Permission>>(false);
                }
            }
        }

        public Feedback<IList<Role>> GetAllRoles()
        {
            using (var entities = new CrsEntities())
            {
                try
                {
                    return new Feedback<IList<Role>>(true, null, entities.Roles.Include("RolePermissions.Permission").ToList());
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new Feedback<IList<Role>>(false);
                }
            }
        }

        public Feedback<Role> InsertRole(Role role, IList<int> permissionIds)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    Role exist = entities.Roles.FirstOrDefault(i => i.Name == role.Name);
                    if (exist != null)
                        return new Feedback<Role>(false, Messages.InsertRole_DuplicateName);

                    // Add to DB
                    entities.Roles.Add(role);
                    if (permissionIds == null)
                        permissionIds = new List<int>();
                    foreach (int itemId in permissionIds)
                    {
                        RolePermission rp = new RolePermission
                        {
                            PermissionId = itemId,
                            RoleId = role.Id
                        };
                        entities.RolePermissions.Add(rp);
                    }

                    entities.SaveChanges();
                }

                return new Feedback<Role>(true, null, role);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Role>(false, Messages.GeneralError);
            }
        }

        public Feedback DeleteRole(int roleId)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Clear data in UserRole table
                    foreach (var userRole in entities.UserRoles.Where(i => i.RoleId == roleId).ToList())
                    {
                        entities.UserRoles.Remove(userRole);
                    }

                    // Clear data in RolePermission table
                    foreach (var a in entities.RolePermissions.Where(t => t.RoleId == roleId).ToList())
                    {
                        entities.RolePermissions.Remove(a);
                    }

                    Role c = entities.Roles.Single(i => i.Id == roleId);
                    entities.Roles.Remove(c);

                    entities.SaveChanges();

                    return new Feedback(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback(false, Messages.GeneralError);
            }
        }

        public Feedback<Role> GetRoleDetails(int roleId)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    Role role = entities.Roles.Include("RolePermissions").SingleOrDefault(i => i.Id == roleId);
                    if (role != null)
                        return new Feedback<Role>(true, null, role);
                    else
                        return new Feedback<Role>(false, Messages.GetRole_NotFound);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Role>(false, Messages.GeneralError);
            }
        }

        public Feedback<Role> UpdateRole(Role c, IList<int> permissionIds)
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    // Check for duplicate name
                    Role exist = entities.Roles.FirstOrDefault(i => i.Name == c.Name && i.Id != c.Id);
                    if (exist != null)
                        return new Feedback<Role>(false, Messages.UpdateRole_DuplicateName);

                    //Remove from RolePermission
                    foreach (var a in entities.RolePermissions.Where(t => t.RoleId == c.Id).ToList())
                        entities.RolePermissions.Remove(a);

                    //Add to DB
                    var role = entities.Roles.Single(i => i.Id == c.Id);
                    role.Name = c.Name;
                    role.Description = c.Description;

                    foreach (int itemId in permissionIds)
                    {
                        RolePermission rp = new RolePermission
                        {
                            PermissionId = itemId,
                            RoleId = role.Id
                        };
                        entities.RolePermissions.Add(rp);
                    }

                    entities.SaveChanges();

                    return new Feedback<Role>(true, null, role);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<Role>(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}