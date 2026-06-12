using System.Collections.Concurrent;

namespace Walmart.Services
{
    public class ClientOrderNotificationService : IClientOrderNotificationService
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _messages = new();

        public void AddMessage(string userId, string message)
        {
            var queue = _messages.GetOrAdd(userId, _ => new ConcurrentQueue<string>());
            queue.Enqueue(message);
        }

        public List<string> TakeMessages(string userId)
        {
            if (!_messages.TryGetValue(userId, out var queue))
                return new List<string>();

            var result = new List<string>();
            while (queue.TryDequeue(out var message))
                result.Add(message);

            return result;
        }
    }
}
