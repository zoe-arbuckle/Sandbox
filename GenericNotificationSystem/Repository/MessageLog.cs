using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Repository
{
    public class MessageLog<T> : IMessageLog<T> where T : IMessage
    {
        private readonly List<(DateTime timestamp, T message)> _log = new();

        public IEnumerable<T> GetAllLogs() => _log.Select(e => e.message);

        public IEnumerable<T> GetRecent(TimeSpan window)
        {
            var cutoff = DateTime.UtcNow - window;
            return _log.Where(entry => entry.timestamp > cutoff)
                .Select(entry => entry.message);
        }

        public void Log(T message) => _log.Add((DateTime.UtcNow, message));
    }
}
