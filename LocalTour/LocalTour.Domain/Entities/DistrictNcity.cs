using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class DistrictNcity
{
    public int Id { get; set; }

    public int ProvinceNcityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ProvinceNcity ProvinceNcity { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<ModTag> ModTags { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
