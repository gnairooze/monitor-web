using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace MonitorWebAppConsole
{
    class Alert
    {
        public static string AlertFromMail { get; set; }
        public static string AlertFromName { get; set; }
        public static string AlertToMail { get; set; }
        public static string AlertToName { get; set; }
        public static string AlertFromPassword { get; set; }
        public static bool DemoMode { get; set; }

        public enum AlertType
        {
            NotSet = 0,
            MonitorStarted = 1,
            SiteDown = 2,
            SiteUp = 4
        }
        public static void SendMail(string subject, string body)
        {
            var fromAddress = new MailAddress(AlertFromMail, AlertFromName);
            var toAddress = new MailAddress(AlertToMail, AlertToName);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, AlertFromPassword)
            };

            if (DemoMode)
            {
                subject = "Demo - " + subject + " - Demo";

                StringBuilder bld = new StringBuilder();
                bld.AppendLine("---This is a demo ---");
                bld.AppendLine("<br/>");
                bld.AppendLine(body);
                bld.AppendLine("<br/>");
                bld.AppendLine("---This is a demo ---");
                body = bld.ToString();
            }

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                try
                {
                    smtp.Send(message);
                    Logger.Log("!!!! Email sent !!!!!");
                }
                catch (Exception ex)
                {
                    Logger.Log("!!! Error In Sending Email !!!");
                    Logger.Log(ex.Message);
                    Logger.Log(ex.StackTrace);

                    ex = ex.InnerException;

                    while (ex != null)
                    {
                        Logger.Log(ex.Message);
                        Logger.Log(ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
                
            }
        }

        public static string ComposeMailBody(AlertType mailType, Dictionary<string, string> parameters = null)
        {
            string mailBody = string.Empty;

            switch (mailType)
            {
                case AlertType.MonitorStarted:
                    mailBody = "Monitor has started";
                    break;
                case AlertType.SiteDown:
                    mailBody = "Site is down";
                    break;
                case AlertType.SiteUp:
                    mailBody = "Site is up";
                    break;
            }

            return mailBody;
        }

        public static string ComposeMailSubject(AlertType mailType, Dictionary<string, string> parameters = null)
        {
            string mailSubject = string.Empty;

            switch (mailType)
            {
                case AlertType.MonitorStarted:
                    mailSubject = "Monitor has started";
                    break;
                case AlertType.SiteDown:
                    mailSubject = "Site is down";
                    break;
                case AlertType.SiteUp:
                    mailSubject = "Site is up";
                    break;
            }

            return mailSubject;
        }
    }
}
