namespace Kookaburra.Email
{
    public interface IEmailSender
    {
        void Send(EmailMessage message);
    }
}