namespace LocalTour.Services.ViewModel;

public class PlaceSearchHistoryVM
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; }
    public string PlacePhotoDisplayUrl { get; set; }
    public DateTime LastSearchDate { get; set; }
}