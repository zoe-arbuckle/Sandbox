using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Builders
{
    public class EmailNotificationBuilder : NotificationBuilder<EmailNotificationBuilder, EmailMessage>
    {
        private string? _recipient;
        private string? _body;

        public EmailNotificationBuilder WithRecipient(string recipient)
        {
            _recipient = recipient;
            return this;
        }

        public EmailNotificationBuilder WithBody(string body)
        {
            _body = body;
            return this;
        }

        public override EmailMessage Build()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(_recipient, nameof(_recipient));
            ArgumentException.ThrowIfNullOrWhiteSpace(_body, nameof(_body));

            return new EmailMessage(_subject, _recipient, _body);
        }
    }
}
