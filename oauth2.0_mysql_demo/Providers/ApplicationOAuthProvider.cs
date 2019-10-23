using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using oauth2._0_mysql_data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace oauth2._0_mysql_demo.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }


        /// <summary>
        /// 客户端授权[生成access token]
        /// </summary>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var signInManager = context.OwinContext.GetUserManager<ApplicationSignInManager>();
            var username = signInManager.GetUserName(context.UserName);
            var statu = signInManager.PasswordSignIn(context.UserName, context.Password, true, true);
            if (statu != SignInStatus.Success)
            {
                switch (statu)
                {
                    case SignInStatus.Failure:
                        context.SetError("invalid_grant", "用户名或密码不正确。");
                        break;
                    case SignInStatus.LockedOut:
                        context.SetError("invalid_grant", "账户被锁定。");
                        break;
                    case SignInStatus.RequiresVerification:
                        context.SetError("invalid_grant", "账户未通过验证。");
                        break;
                }
                return;
            }
            var user = signInManager.UserManager.FindByName(username);
            user.LastLogin = DateTime.Now;
            await signInManager.UserManager.UpdateAsync(user);

            ClaimsIdentity oAuthIdentity = await signInManager.UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await signInManager.UserManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user, context);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }


        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // 资源所有者密码凭据未提供客户端 ID。
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }


        public static AuthenticationProperties CreateProperties(IdentityUser user, OAuthGrantResourceOwnerCredentialsContext context)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "id", user.Id}
            };

            if (!string.IsNullOrWhiteSpace(user.RealName))
                data["realname"] = user.RealName;

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                data["phone"] = user.PhoneNumber;

            // 获取权限，是否具有系统管理员权限
            if (!string.IsNullOrEmpty(user.Roles) && user.Roles.Contains("SystemAdmin"))
            {
                data["adminsys"] = "true";
            }

            return new AuthenticationProperties(data);
        }

    }
}