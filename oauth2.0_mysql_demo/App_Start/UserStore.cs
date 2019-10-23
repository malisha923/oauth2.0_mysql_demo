using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using oauth2._0_mysql_data;

namespace oauth2._0_mysql_demo
{
    public class UserStore<TUser> : IUserStore<TUser>,
                                    IUserPasswordStore<TUser>,
                                    IUserRoleStore<TUser>,
                                    IUserSecurityStampStore<TUser>,
                                    IUserEmailStore<TUser>,
                                    IUserPhoneNumberStore<TUser>,
                                    IUserTwoFactorStore<TUser, string>,
                                    IUserLockoutStore<TUser, string>
                                    where TUser : IdentityUser
    {

        private bool _disposed;


        public UserStore()
        {
        }


        public virtual void Dispose()
        {
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }


        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var mysqldb = new MysqlContext())
            {
                mysqldb.Users.Add(user);
                return mysqldb.SaveChangesAsync();
            }
        }


        /// <summary>
        /// 编辑记录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var mysqldb = new MysqlContext())
            {
                mysqldb.Users.AddOrUpdateExtension(user);
                return mysqldb.SaveChangesAsync();
            }
        }


        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var mysqldb = new MysqlContext())
            {
                var u = mysqldb.Users.Where(o => o.Id == user.Id).FirstOrDefault();
                if (u == null)
                {
                    throw new ArgumentNullException("user");
                }
                else
                {
                    mysqldb.Entry(u).State = EntityState.Deleted;
                    return mysqldb.SaveChangesAsync();
                }
            }
        }


        /// <summary>
        /// 按手机号查找用户
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByPhoneAsync(string mobile)
        {
            ThrowIfDisposed();
            using (var mysqldb = new MysqlContext())
            {
                return mysqldb.Users.Where(o => o.PhoneNumber == mobile).Cast<TUser>().FirstOrDefaultAsync();
            }
        }


        /// <summary>
        /// 通过用户Id查找用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();
            using (var mysqldb = new MysqlContext())
            {
                return mysqldb.Users.Where(o => o.Id == userId).Cast<TUser>().FirstOrDefaultAsync();
            }
        }


        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            using (var mysqldb = new MysqlContext())
            {
                return mysqldb.Users.Where(o => o.UserName == userName).Cast<TUser>().FirstOrDefaultAsync();
            }
        }


        /// <summary>
        /// 通过邮箱查找用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            using (var mysqldb = new MysqlContext())
            {
                return mysqldb.Users.Where(o => o.Email == email).Cast<TUser>().FirstOrDefaultAsync();
            }
        }


        public virtual Task AddToRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var mysqldb = new MysqlContext())
            {
                var u = mysqldb.Users.Where(o => o.Id == user.Id).FirstOrDefault();
                if (u == null)
                {
                    throw new ArgumentNullException("user");
                }
                else
                {
                    if (string.IsNullOrEmpty(user.Roles))
                    {
                        user.Roles = roleName;
                    }
                    else
                    {
                        user.Roles = user.Roles + "," + roleName;
                    }
                    mysqldb.Entry(user).State = EntityState.Modified;
                    return mysqldb.SaveChangesAsync();
                }
            }
        }


        public virtual Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            using (var mysqldb = new MysqlContext())
            {
                var u = mysqldb.Users.Where(o => o.Id == user.Id).FirstOrDefault();
                if (u == null)
                {
                    throw new ArgumentNullException("user");
                }
                else
                {
                    if ((!string.IsNullOrEmpty(user.Roles)) && (user.Roles.Contains(roleName)))
                    {
                        List<string> t_roles = new List<string>(user.Roles.Split(','));
                        t_roles.Remove(roleName);
                        user.Roles = string.Join(",", t_roles);
                    }
                    mysqldb.Entry(user).State = EntityState.Modified;
                    return mysqldb.SaveChangesAsync();
                }
            }
        }


        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult((IList<string>)user.Roles.Split(','));
        }


        /// <summary>
        /// 是否拥有该权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.FromResult(user.Roles.Contains(roleName));
        }


        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }


        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }


        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(user.HasPassword());
        }


        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }


        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }


        public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }


        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }


        public virtual Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }


        public virtual Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }


        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }


        public virtual Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }


        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }


        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }


        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }


        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }


        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEndDateUtc ?? new DateTimeOffset());
        }


        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = new DateTime(lockoutEnd.Ticks, DateTimeKind.Utc);
            return Task.FromResult(0);
        }


        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }


        public virtual Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }


        public virtual Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }


        public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }


        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }


        public virtual async Task<bool> IsLockedOutAsync(TUser user)
        {
            var lockoutTime = await GetLockoutEndDateAsync(user);
            return lockoutTime >= DateTimeOffset.UtcNow;
        }

    }
}