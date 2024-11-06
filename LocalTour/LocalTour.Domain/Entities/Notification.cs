using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string NotificationType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime TimeSend { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = null!;
}
