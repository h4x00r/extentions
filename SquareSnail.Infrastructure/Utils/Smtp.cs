
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace SquareSnail.Infrastructure
{
    public static class Smtp
    {
        public static void SendMail(string to, string from, string subject, string body, string[] cc)
        {
            string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

            SmtpClient client = new SmtpClient(smtpServer);

            client.UseDefaultCredentials = false;
            client.Timeout = 20000;
            client.Port = 25;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            MailMessage mail = new MailMessage(from, to);

            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;

            if (cc != null)
            {
                for (int i = 0; i < cc.Length; i++)
                {
                    mail.CC.Add(cc[i]);
                }
            }

            client.Send(mail);
        }
        public static void SendMail(string to, string from, string subject, string body, string[] cc,
            string smptServer, string username, string password)
        {
            SmtpClient client = new SmtpClient(smptServer);

            client.UseDefaultCredentials = false;
            client.Timeout = 20000;
            client.Port = 25;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(username, password);

            MailMessage mail = new MailMessage(from, to);

            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;

            if (cc != null)
            {
                for (int i = 0; i < cc.Length; i++)
                {
                    mail.CC.Add(cc[i]);
                }
            }

            client.Send(mail);
        }
    }
}
