﻿using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceActivity
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public int DisplayNumber { get; set; }

    public string? PhotoDisplay { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual ICollection<PlaceActivityPhoto> PlaceActivityPhotos { get; set; } = new List<PlaceActivityPhoto>();

    public virtual ICollection<PlaceActivityTranslation> PlaceActivityTranslations { get; set; } = new List<PlaceActivityTranslation>();

    public virtual ICollection<PlaceActivityVideo> PlaceActivityVideos { get; set; } = new List<PlaceActivityVideo>();
}