using RazorEngine;
using RazorEngine.Templating;
using System.IO;
using System.Reflection;

namespace Kookaburra.Email
{
    public class Mailer : IMailer
    {
        private readonly IEmailSender _emailSender;

        public Mailer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void SendEmail<T>(AddressInfo from, AddressInfo to, T model) where T : IEmailModel
        {            
            var body = AssembleMessageBody(model);
            var subject = GetSubject(body);

            var message = new EmailMessage
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            _emailSender.Send(message);
        }

        private string GetTemplate<T>(T model) where T : IEmailModel
        {
            var assembly = Assembly.GetExecutingAssembly();            

            var resourceName = $"{model.GetType().FullName}.html";

            string template = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }

        private string AssembleMessageBody<T>(T model) where T : IEmailModel
        {
            var template = GetTemplate(model);
            return Engine.Razor.RunCompile(template, model.GetType().Name, null, model);
        }

        private string GetSubject(string processedTemplate)
        {
            int indexOfTitleText = processedTemplate.ToLower().IndexOf("<title>") + 7; //Adding 8 to get to start of actual text
            int lastIndexOfTitle = processedTemplate.ToLower().IndexOf("</title>", indexOfTitleText);

            return processedTemplate.Substring(indexOfTitleText, lastIndexOfTitle - indexOfTitleText);
        }
    }
}