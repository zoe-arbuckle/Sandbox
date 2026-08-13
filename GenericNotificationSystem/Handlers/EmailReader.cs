using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Handlers
{
    public class EmailReader : IMessageReader<EmailMessage>
    {
        private readonly Queue<EmailMessage> _messages = new(
        [
            new EmailMessage("Invoice", "a@b.com",  "See attached"),
            new EmailMessage("Hello", "b@c.com", "Hello there")
        ]);
        public EmailMessage ReadNext() => _messages.Dequeue(); // replace with message  source
    }
}
