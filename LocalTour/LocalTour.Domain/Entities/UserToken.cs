using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class UserToken
{
    public Guid UserId { get; set; }

    public Guid LoginProvider { get; set; }

    public Guid Name { get; set; }

    public string? Value { get; set; }

    public virtual User User { get; set; } = null!;
}
