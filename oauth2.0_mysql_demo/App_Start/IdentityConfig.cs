using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using oauth2._0_mysql_data;

namespace oauth2._0_mysql_demo
{
    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<IdentityUser>
    {
        public ApplicationUserManager(IUserStore<IdentityUser> store) : base(store)
        {
        }


        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<IdentityUser>());
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<IdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // 配置用户锁定默认值
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<IdentityUser, string>
            {
                MessageFormat = ConfigurationManager.AppSettings["SMSTemplate.Code"]
            });
            manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<IdentityUser, string>
            {
                Subject = "安全代码",
                BodyFormat = "你的安全代码是 {0}"
            });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }


        /// <summary>
        /// 通过手机号查找用户
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Task<IdentityUser> FindByPhoneAsync(string mobile)
        {
            var u = ((UserStore<IdentityUser>)this.Store).FindByPhoneAsync(mobile);
            return u;
        }
    }


    public class ApplicationSignInManager : SignInManager<IdentityUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }


        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }


        // 验证手机号正则式
        public readonly static Regex phoneRegex = new Regex(@"^([0-9\(\)\/\+ \-]*)$", RegexOptions.Compiled);
        // 验证邮箱正则式
        public readonly static Regex emailRegex = new Regex("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        /// <summary>
        /// 使用密码登录，支持用户名/邮箱/手机号
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            userName = GetUserName(userName);

            if (string.IsNullOrWhiteSpace(userName))
                return SignInStatus.Failure;

            return await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }


        /// <summary>
        /// 通过用户名/邮箱/手机号，获取登录邮箱
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            userName = userName.Trim();

            //验证邮箱
            if (emailRegex.IsMatch(userName))
            {
                var u = UserManager.FindByEmailAsync(userName).GetAwaiter().GetResult();
                userName = u != null ? u.UserName : null;
            }
            else
            {
                //验证手机号
                var u = ((ApplicationUserManager)UserManager).FindByPhoneAsync(userName).GetAwaiter().GetResult();
                if (u != null)
                {
                    userName = u.UserName;
                }
            }
            return userName;
        }


    }

}
