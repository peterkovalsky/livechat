using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public interface IEmailService
    {
        void SendForgorPasswordEmail(string email, string code, string host);

        Task SendSignUpWelcomeEmailAsync(string operatorIdentity);

        void SendOfflineNotificationEmail(long messageId);
    }
}