using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class ProvinceNcity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DistrictNcity> DistrictNcities { get; set; } = new List<DistrictNcity>();
}
