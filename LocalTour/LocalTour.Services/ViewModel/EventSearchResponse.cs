namespace LocalTour.Services.ViewModel;

public class EventSearchResponse
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; }
    public string PlacePhoto  { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string EventPhoto  { get; set; }
    public string EventName { get; set; } = null!;
    public double Distance { get; set; }
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string EventStatus { get; set; } = null!;
}