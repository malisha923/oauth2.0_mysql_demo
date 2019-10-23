using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace oauth2._0_mysql_data
{
    [Table("Users")]
    public class IdentityUser : IUser<string>
    {
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 记录Id
        /// </summary>
        [Key]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// 登录账号（邮箱）
        /// </summary>
        public string UserName
        {
            get; set;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName
        {
            get; set;
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public bool EmailConfirmed
        {
            get; set;
        }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber
        {
            get; set;
        }

        public bool PhoneNumberConfirmed
        {
            get; set;
        }

        public string SecurityStamp
        {
            get; set;
        }

        public bool TwoFactorEnabled
        {
            get; set;
        }

        public DateTime? LockoutEndDateUtc
        {
            get; set;
        }

        public DateTime? LastLogin
        {
            set; get;
        }

        public bool LockoutEnabled
        {
            get; set;
        }

        public int AccessFailedCount
        {
            get; set;
        }

        public bool NotifacationEmailClosed
        {
            set; get;
        }

        public bool NotifacationSMSClosed
        {
            set; get;
        }

        public string Roles
        {
            get; set;
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string PasswordHash
        {
            get; set;
        }

        public virtual bool HasPassword()
        {
            return false;
        }

        /// <summary>
        /// 微信绑定用Id
        /// </summary>
        public string OpenId
        {
            get; set;
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager, string authenticationType)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // 在此处添加自定义用户声明
            return userIdentity;
        }

    }


    /// <summary>
    /// 性别
    /// </summary>
    public enum Sex
    {
        None,
        Male,
        Female
    }

}
