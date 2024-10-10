using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Weather
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string? Location { get; set; }

    public virtual User User { get; set; } = null!;
}
