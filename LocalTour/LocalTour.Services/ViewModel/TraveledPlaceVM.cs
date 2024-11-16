using LocalTour.Domain.Entities;

namespace LocalTour.Services.ViewModel;

public class TraveledPlaceVM
{ 
    public PlaceTraveledVM Place { get; set; }
    public int TraveledTimes { get; set; }
}

public class PlaceTraveledVM
{
    public string PlaceName { get; set; }
    public string PlacePhotoDisplay { get; set; }
    public string WardName { get; set; }
    public DateTime FirstVisitDate { get; set; }
    public DateTime LastVisitDate { get; set; }
}