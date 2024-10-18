﻿using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceActivityTranslation
{
    public int Id { get; set; }

    public int PlaceActivityId { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string ActivityName { get; set; } = null!;

    public double Price { get; set; }

    public string? Description { get; set; }

    public string PriceType { get; set; } = null!;

    public double? Discount { get; set; }

    public virtual PlaceActivity PlaceActivity { get; set; } = null!;
}