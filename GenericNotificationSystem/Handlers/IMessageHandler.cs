using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Handlers
{
    public interface IMessageHandler<in T> where T : IMessage
    {
        DeliveryResult Handle(T message);
    }
}
