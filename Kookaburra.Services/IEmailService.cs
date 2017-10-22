namespace Kookaburra.Services
{
    public interface IEmailService
    {
        void SendOfflineNotificationEmail(long messageId);
    }
}