using LocalTour.Domain.Entities;

namespace LocalTour.Services.ViewModel;

public class PlaceDetailModel : Place
{
  public double Rating { get; set; }
}