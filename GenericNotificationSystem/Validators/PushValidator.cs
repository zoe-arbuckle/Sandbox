using GenericNotificationSystem.Models;

namespace GenericNotificationSystem.Validators
{
    public class PushValidator : MessageValidator<PushMessage>
    {
        protected override IEnumerable<string> ValidateMessage(PushMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.DeviceToken))
                yield return "Device Token is required";
        }
    }
}
