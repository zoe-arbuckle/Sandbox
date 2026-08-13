using GenericNotificationSystem.Models;
using System.Text.RegularExpressions;

namespace GenericNotificationSystem.Validators
{
    public class EmailValidator : MessageValidator<EmailMessage>
    {
        private static readonly Regex EmailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        protected override IEnumerable<string> ValidateMessage(EmailMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.To) || !EmailPattern.IsMatch(message.To))
                yield return "Recipient email address is required";
            if (string.IsNullOrWhiteSpace(message.Body))
                yield return "Email body is required";
        }
    }
}
