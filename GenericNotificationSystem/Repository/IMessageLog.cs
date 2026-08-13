using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Repository
{
    public interface IMessageLog<T> where T : IMessage
    {
        void Log(T message);
        IEnumerable<T> GetAllLogs();
        IEnumerable<T> GetRecent(TimeSpan window);
    }
}
