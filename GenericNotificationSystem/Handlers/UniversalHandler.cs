using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Handlers
{
    public class UniversalHandler : IMessageHandler<IMessage>
    {
        public DeliveryResult Handle(IMessage message)
        {
            // Replace with delivery logic
            Console.WriteLine($"Handling message {message.GetType()}: {message.Subject}");
            return new DeliveryResult(true, "Universal");
        }
    }
}
