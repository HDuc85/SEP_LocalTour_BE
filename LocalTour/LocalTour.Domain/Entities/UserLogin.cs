using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class UserLogin
{
    public Guid LoginProvider { get; set; }

    public Guid ProviderKey { get; set; }

    public string? ProviderDisplayName { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
