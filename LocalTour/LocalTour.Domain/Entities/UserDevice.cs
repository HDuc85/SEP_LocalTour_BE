using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class UserDevice
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string DeviceId { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
