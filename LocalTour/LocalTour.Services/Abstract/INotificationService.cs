namespace LocalTour.Services.Abstract;

public interface INotificationService
{
    Task<List<Domain.Entities.Notification>> GetAll(string userId);
    Task<bool> ReadedNotification(string userId, int notificationId);
    Task<bool> DeleteNotification(int notificationId, string userId);
    Task<string> SetNotificationForEvent(int eventId, string title, string body);
    Task<string> SetNotificationForSystem(string userId, string title, string body, DateTime timeSend);
    Task<string> ScheduleNotification(List<string> deviceTokens,int NotificationId, string title, string body, DateTime scheduleTime);
    Task<string> SendNotificationNow(string userId, string deviceToken, string title, string body, string notificationType);
    Task<string> AddDeviceToken(string userId, string deviceToken);
    Task<string> DeleteNotification(string userId, string deviceToken);
}