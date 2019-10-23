using CDO;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace oauth2._0_mysql_demo
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                var config = ConfigurationManager.AppSettings.LoadConfig<EmailServiceConfig>("EmailService.");
                CDO.Message oMsg = new CDO.Message();
                oMsg.Configuration.Fields[CdoConfiguration.cdoSendUsingMethod].Value = CdoSendUsing.cdoSendUsingPort;
                oMsg.Configuration.Fields[CdoConfiguration.cdoSMTPAuthenticate].Value = CdoProtocolsAuthentication.cdoBasic;
                if (config.SSL != null)
                    oMsg.Configuration.Fields[CdoConfiguration.cdoSMTPUseSSL].Value = config.SSL;
                oMsg.Configuration.Fields[CdoConfiguration.cdoSMTPServer].Value = config.Host;//smtp服务器地址
                if (config.Port != null)
                    oMsg.Configuration.Fields[CdoConfiguration.cdoSMTPServerPort].Value = config.Port;//邮箱端口，不设置则为：25
                oMsg.Configuration.Fields[CdoConfiguration.cdoSendEmailAddress].Value = config.SenderAddress;//发送者邮箱
                oMsg.Configuration.Fields[CdoConfiguration.cdoSendUserName].Value = config.Account;     //发送者邮箱   
                oMsg.Configuration.Fields[CdoConfiguration.cdoSendPassword].Value = config.Password;    //邮箱发送者密码
                oMsg.Configuration.Fields.Update();
                //oMsg.TextBody = body;//邮件正文
                oMsg.HTMLBody = message.Body;
                oMsg.Subject = message.Subject;//主题
                oMsg.From = string.Format("{0}<{1}>", config.SenderName, config.SenderAddress);
                oMsg.To = message.Destination;//接收者
                oMsg.Send();//发送
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oMsg);
                oMsg = null;
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }


        private class EmailServiceConfig
        {
            public string SenderAddress { get; set; }

            public string SenderName { get; set; }

            public string Account { get; set; }

            public string Password { get; set; }

            public string Host { get; set; }

            public string Port { get; set; }

            public string SSL { get; set; }
        }

    }
}