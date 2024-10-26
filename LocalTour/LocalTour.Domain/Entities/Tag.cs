using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Tag
{
    public int Id { get; set; }

    public string TagName { get; set; } = null!;

    public string TagPhotoUrl { get; set; } = null!;

    public virtual ICollection<PlaceTag> PlaceTags { get; set; } = new List<PlaceTag>();
    public virtual ICollection<UserPreferenceTags> UserPreferenceTags { get; set; } = new List<UserPreferenceTags>();

}
