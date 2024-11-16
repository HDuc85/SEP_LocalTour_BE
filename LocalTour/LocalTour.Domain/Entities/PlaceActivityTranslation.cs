using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public virtual PlaceActivity PlaceActivity { get; set; } = null!;
}
