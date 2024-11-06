namespace LocalTour.Domain.Entities;

public partial class UserNotification
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    
    public int NotificationId { get; set; }
    
    public bool IsReaded { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Notification Notification { get; set; } = null!;
}