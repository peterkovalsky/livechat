using System.Net;
using System.Net.Mail;
using System.Text;

namespace Kookaburra.Email
{
    public class DefaultEmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public DefaultEmailSender(string host, string username, string password)
        {
            _smtpClient = new SmtpClient
            {
                Host = host,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };
        }

        public void Send(EmailMessage message)
        {
            if (message == null) return;

            var mailMessage = new MailMessage(new MailAddress(message.From, message.DisplayName), new MailAddress(message.To));

            mailMessage.Body = message.Body;
            mailMessage.Subject = message.Subject;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            mailMessage.IsBodyHtml = message.IsHtml;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            _smtpClient.Send(mailMessage);
        }
    }
}