using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class Ward
{
    public int Id { get; set; }

    public int DistrictNcityId { get; set; }

    public string WardName { get; set; } = null!;

    public virtual DistrictNcity DistrictNcity { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
