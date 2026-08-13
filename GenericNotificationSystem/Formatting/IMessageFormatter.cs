using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Formatting
{
    public interface IMessageFormatter<in T> where T : IMessage
    {
        string Format(T message);
    }
}
