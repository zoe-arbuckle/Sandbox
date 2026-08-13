using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Specifications
{
    public class SubjectContainsSpec(string keyword) : ISpecification<IMessage>
    {
        public bool IsSatisfiedBy(IMessage message) => message.Subject.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

}
