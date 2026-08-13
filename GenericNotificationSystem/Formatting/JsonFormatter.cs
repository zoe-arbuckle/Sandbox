using GenericNotificationSystem.Models;
using System.Text.Json;

namespace GenericNotificationSystem.Formatting
{
    public class JsonFormatter<T> : IMessageFormatter<T> where T : IMessage
    {
        public string Format(T message)
        {
            return JsonSerializer.Serialize(message);
        }
    }
}
