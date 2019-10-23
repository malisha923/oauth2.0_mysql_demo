using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace oauth2._0_mysql_demo
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                var url = BuildUrl(message);
                using (WebClient client = new WebClient())
                {
                    return client.DownloadStringTaskAsync(url);
                }
            }
            catch
            {
                return Task.FromResult(0);
            }
        }


        public static string BuildUrl(IdentityMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Destination) || (string.IsNullOrWhiteSpace(message.Subject) && string.IsNullOrWhiteSpace(message.Body)))
                throw new ArgumentNullException();

            var config = ConfigurationManager.AppSettings.LoadConfig<SmsServiceConfig>("SmsService.");

            message.Destination = message.Destination.Trim();
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("srcmobile", config.Account);
            parms.Add("password", config.Password);

            var d = message.Destination.Split(" ,;，；　\t".ToArray(), StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            if (d.Length == 0)
                throw new ArgumentNullException();
            if (d.Length == 0)
            {
                parms.Add("objmobile", message.Destination);
                parms.Add("smsid", string.Format("{0}{1:yyyyMMddHHmmssfff}", message.Destination, DateTime.Now));
            }
            else
            {
                var ds = string.Join(";", d.Select(o => string.Format("{0},{0}{1:yyyyMMddHHmmssfff}", o, DateTime.Now)));
                parms.Add("objmobiles", ds);
            }
            var txt = string.Format("{0}{1}", message.Subject, message.Body).Trim();
            if (txt.Length > 140)
                txt = txt.Substring(0, 140);
            parms.Add("smstext", txt);
            parms.Add("rstype", "text");

            return string.Format("{0}?{1}", config.Url, string.Join("&", parms.Select(o => string.Format("{0}={1}", o.Key, HttpUtility.UrlEncode(o.Value)))));
        }


        private class SmsServiceConfig
        {
            public string Account { get; set; }

            public string Password { get; set; }

            public string Url { get; set; }
        }

    }
}