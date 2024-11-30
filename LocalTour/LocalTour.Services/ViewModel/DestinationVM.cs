namespace LocalTour.Services.ViewModel;

public class DestinationVM
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; }
    public string PlacePhotoDisplay { get; set; }
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Detail { get; set; }
    public bool IsArrived { get; set; }
}