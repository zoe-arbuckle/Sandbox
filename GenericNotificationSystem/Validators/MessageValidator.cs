using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Validators
{
    public abstract class MessageValidator<T> where T : class, IMessage
    {
        public IEnumerable<string> Validate(T message)
        {
            if (string.IsNullOrWhiteSpace(message.Subject))
                yield return "Subject is required";

            foreach (var error in ValidateMessage(message))
                yield return error;
        }

        protected abstract IEnumerable<string> ValidateMessage(T message);

        public static TValidator Create<TValidator>()
            where TValidator : MessageValidator<T>, new()
        {
            return new TValidator();
        }
    }
}
