using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using Microsoft.AspNetCore.Http;

namespace LocalTour.Services.ViewModel;

public class PlaceRequest : IMapFrom<Place>
{
    //public int WardId { get; set; }

    public string PhotoDisplay { get; set; } = null!;

    public TimeOnly TimeOpen { get; set; }

    public TimeOnly TimeClose { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }
    public List<int> Tags { get; set; }
    public List<IFormFile> PlacePhotos { get; set; }
    public List<PlaceTranslationRequest> PlaceTranslation { get; set; }

}

