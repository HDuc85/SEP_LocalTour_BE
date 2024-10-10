using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class UserBan
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime EndDate { get; set; }

    public virtual User User { get; set; } = null!;
}
