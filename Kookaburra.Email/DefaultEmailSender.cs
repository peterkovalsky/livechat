using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };
        }

        public void Send(EmailMessage message)
        {
            if (message == null) return;

            var mailMessage = new MailMessage(new MailAddress(message.From.Email, message.From.DisplayName), new MailAddress(message.To.Email, message.To.DisplayName))
            {
                Body = message.Body,
                Subject = message.Subject,
                BodyEncoding = UTF8Encoding.UTF8,
                IsBodyHtml = message.IsHtml,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };

            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    var contentType = new ContentType
                    {
                        MediaType = attachment.MIME,
                        Name = attachment.Filename
                    };
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(attachment.File), contentType));
                }
            }

            _smtpClient.Send(mailMessage);
        }
    }
}