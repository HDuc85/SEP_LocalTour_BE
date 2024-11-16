using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class PlaceTranslation
{
    public int Id { get; set; }

    public int PlaceId { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Address { get; set; } = null!;

    public string? Contact { get; set; }
    [JsonIgnore]
    public virtual Place Place { get; set; } = null!;
}
