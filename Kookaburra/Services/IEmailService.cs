namespace Kookaburra.Services
{
    public interface IEmailService
    {
        void SendSignUpWelcomeEmail(string operatorIdentity);

        void SendOfflineMessageEmail(long id);
    }
}