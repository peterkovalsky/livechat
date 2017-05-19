namespace Kookaburra.Email
{
    public interface IMailer
    {
        void SendEmail<T>(AddressInfo from, AddressInfo to, T model) where T : IEmailModel;
    }
}