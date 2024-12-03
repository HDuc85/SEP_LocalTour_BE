namespace LocalTour.Services.ViewModel;

public class TraveledPlaceViewModel
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; }
    public string PlacePhotoDisplay { get; set; }
    public int vistitedTimes { get; set; }
    public DateTime firstVisitTime { get; set; }
    public DateTime lastVisitTime { get; set; }
}