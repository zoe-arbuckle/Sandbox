using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Specifications
{
    public class MessageTypeSpec<T> :  ISpecification<IMessage> where T : IMessage
    {
        public bool IsSatisfiedBy(IMessage message) => message is T;
    }

}
