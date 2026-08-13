using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Handlers
{
    public interface IMessageReader<out T> where T : IMessage
    {
        T ReadNext();
    }
}
