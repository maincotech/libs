using System.Net.Mail;
using System.Text;

namespace System.Net
{
    #region MailHostType

    /// <summary>
    ///
    /// </summary>
    public enum MailHostType
    {
        /// <summary>
        ///
        /// </summary>
        LocalHost,

        /// <summary>
        ///
        /// </summary>
        Smtp,

        /// <summary>
        ///
        /// </summary>
        SmtpUseSsl
    }

    #endregion MailHostType

    #region MailHostConfig

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class MailHostConfig
    {
        public MailHostType HostType { get; set; }

        public string HostAddress { get; set; }

        public int HostPort { get; set; }
    }

    #endregion MailHostConfig

    /// <summary>
    /// EmailSender
    /// </summary>
    public static class EmailSender
    {
        #region SendGMail

        /// <summary>
        /// gmail config
        /// </summary>
        private static readonly MailHostConfig GmailHostAddress = new MailHostConfig
        {
            HostType = MailHostType.SmtpUseSsl,
            HostAddress = "smtp.gmail.com",
            HostPort = 587
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mailAdress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool SendGMail(MailMessage msg, string mailAdress, string password)
        {
            return SendMail(GmailHostAddress, msg, mailAdress, password);
        }

        #endregion SendGMail

        #region SendMail

        /// <summary>
        ///
        /// </summary>
        /// <param name="hostConfig"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SendMail(MailHostConfig hostConfig, MailMessage msg, string mailAdress, string password)
        {
            Check.Require(hostConfig, "hostConfig");
            Check.Require(hostConfig.HostAddress, "hostConfig.HostAddress");
            Check.Require(hostConfig.HostType, "hostConfig.HostType");
            Check.Require(hostConfig.HostPort >= 0);

            Check.Require(msg, "msg");

            Check.Require(mailAdress, "mailAdress", Check.IsEmailAddress);

            if (msg.To.Count == 0)
            {
                return false;
            }

            var client = new SmtpClient();

            switch (hostConfig.HostType)
            {
                default:
                    client.Host = "localhost";
                    break;

                case MailHostType.Smtp:
                    client.Credentials = new NetworkCredential(mailAdress, password);
                    client.Host = hostConfig.HostAddress;
                    break;

                case MailHostType.SmtpUseSsl:
                    client.Credentials = new NetworkCredential(mailAdress, password);
                    client.Port = hostConfig.HostPort;
                    client.Host = hostConfig.HostAddress;
                    client.EnableSsl = true;
                    break;
            }

            msg.SubjectEncoding = Encoding.UTF8;
            msg.BodyEncoding = Encoding.UTF8;
            //msg.Priority = MailPriority.High;

            object userState = msg;

            try
            {
                client.SendAsync(msg, userState);

                return true;
            }
            catch //(System.Net.Mail.SmtpException ex)
            {
                return false;
            }
        }

        #endregion SendMail

        #region GetDomainByEmail

        /// <summary>
        ///  Get the domain part of an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetDomainByEmail(string email)
        {
            Check.Require(email, "email", Check.IsEmailAddress);

            var index = email.IndexOf('@');
            return email.Substring(index + 1);
        }

        #endregion GetDomainByEmail
    }
}