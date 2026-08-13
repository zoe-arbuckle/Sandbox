namespace GenericNotificationSystem.Models
{
    public record PushMessage(string Subject, string DeviceToken) : IMessage;
}
