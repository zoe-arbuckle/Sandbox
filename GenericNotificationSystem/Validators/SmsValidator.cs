using GenericNotificationSystem.Models;
using System.Text.RegularExpressions;

namespace GenericNotificationSystem.Validators
{
    public class SmsValidator : MessageValidator<SmsMessage>
    {
        private static readonly Regex PhoneNumberPattern = new Regex(@"^\+?\d{10,15}$", RegexOptions.Compiled);
        protected override IEnumerable<string> ValidateMessage(SmsMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.PhoneNumber) || !PhoneNumberPattern.IsMatch(message.PhoneNumber))
                yield return "Invalid phone number";
            if (string.IsNullOrWhiteSpace(message.Text))
                yield return "SMS text is required";
        }
    }
}
