using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Place
{
    public int Id { get; set; }

    public int WardId { get; set; }

    public string PhotoDisplay { get; set; } = null!;

    public TimeOnly TimeOpen { get; set; }

    public TimeOnly TimeClose { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public string Status { get; set; }

    public virtual ICollection<Destination> Destinations { get; set; } = new List<Destination>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<MarkPlace> MarkPlaces { get; set; } = new List<MarkPlace>();

    public virtual ICollection<PlaceActivity> PlaceActivities { get; set; } = new List<PlaceActivity>();

    public virtual ICollection<PlaceFeeedback> PlaceFeeedbacks { get; set; } = new List<PlaceFeeedback>();

    public virtual PlacePhoto? PlacePhoto { get; set; }

    public virtual ICollection<PlaceSearchHistory> PlaceSearchHistories { get; set; } = new List<PlaceSearchHistory>();

    public virtual ICollection<PlaceTag> PlaceTags { get; set; } = new List<PlaceTag>();

    public virtual ICollection<PlaceTranslation> PlaceTranslations { get; set; } = new List<PlaceTranslation>();

    public virtual ICollection<PlaceVideo> PlaceVideos { get; set; } = new List<PlaceVideo>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<TraveledPlace> TraveledPlaces { get; set; } = new List<TraveledPlace>();

    public virtual Ward Ward { get; set; } = null!;
}
