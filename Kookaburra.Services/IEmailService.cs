using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public interface IEmailService
    {
        Task SendSignUpWelcomeEmailAsync(string operatorIdentity);

        void SendOfflineNotificationEmail(long messageId);
    }
}