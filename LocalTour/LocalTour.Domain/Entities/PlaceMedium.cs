﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class PlaceMedium
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime CreateDate { get; set; }
    [JsonIgnore]
    public virtual Place Place { get; set; } = null!;
}
