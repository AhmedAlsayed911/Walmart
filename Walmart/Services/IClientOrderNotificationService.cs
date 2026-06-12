namespace Walmart.Services
{
    public interface IClientOrderNotificationService
    {
        void AddMessage(string userId, string message);
        List<string> TakeMessages(string userId);
    }
}
