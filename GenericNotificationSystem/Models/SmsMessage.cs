namespace GenericNotificationSystem.Models
{
    public record SmsMessage(string Subject, string PhoneNumber, string Text) : IMessage;
}
