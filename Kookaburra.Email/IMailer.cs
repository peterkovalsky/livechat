namespace Kookaburra.Email
{
    public interface IMailer
    {
        void SendEmail<T>(AddressInfo from, AddressInfo to, T model, string bcc = null) where T : IEmailModel;

        void SendEmail<T>(AddressInfo from, AddressInfo to, string replyTo, T model, string bcc = null) where T : IEmailModel;
    }
}