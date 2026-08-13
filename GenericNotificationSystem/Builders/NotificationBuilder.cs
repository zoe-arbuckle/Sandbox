using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Builders
{
    public abstract class NotificationBuilder<TBuilder, TMessage>
        where TBuilder : NotificationBuilder<TBuilder, TMessage>
        where TMessage : IMessage
    {
        protected string _subject = string.Empty;

        public TBuilder WithSubject(string subject)
        {
            _subject = subject;
            return (TBuilder)this;
        }

        public abstract TMessage Build();
    }
}
