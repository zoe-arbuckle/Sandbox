using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Formatting
{
    public class PlainTextFormatter<T> : IMessageFormatter<T> where T : IMessage
    {
        public string Format(T message)
        {
            return $"[{message.GetType().Name}] Subject: {message.Subject}";
        }
    }
}
