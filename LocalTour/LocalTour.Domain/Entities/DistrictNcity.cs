using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class DistrictNcity
{
    public int Id { get; set; }

    public int ProvinceNcityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ProvinceNcity ProvinceNcity { get; set; } = null!;

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
