namespace GenericNotificationSystem.Models
{
    public record DeliveryResult(bool Success, string Channel, string? Error = null);
}
