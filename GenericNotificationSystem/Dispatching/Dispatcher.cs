using GenericNotificationSystem.Formatting;
using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Dispatching
{
    public class Dispatcher<T>(IMessageFormatter<T> formatter) where T: IMessage
    {
        public bool Dispatch(T message)
        {
            var formattedMessage = formatter.Format(message);
            // Do something to dispatch the message
            Console.WriteLine(formattedMessage);
            return true;
        }
    }
}
