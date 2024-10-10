using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class FollowUser
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid UserFollow { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User UserFollowNavigation { get; set; } = null!;
}
