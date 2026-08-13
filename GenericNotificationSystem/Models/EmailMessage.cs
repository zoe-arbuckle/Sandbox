namespace GenericNotificationSystem.Models
{
    public record EmailMessage(string Subject, string To, string Body) : IMessage;
}
