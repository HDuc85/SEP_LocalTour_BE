using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using LocalTour.Services.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalTour.Services.ViewModel;

public class PlaceRequest 
{
    public int WardId { get; set; }

    public IFormFile PhotoDisplay { get; set; } = null!;

    public TimeOnly TimeOpen { get; set; }

    public TimeOnly TimeClose { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }
    public string ContactLink { get; set; }
    public List<int> Tags { get; set; }
    
    public string? BRC { get; set; }
    public List<IFormFile>? PlaceMedia { get; set; }
    [ModelBinder(BinderType = typeof(FromJsonBinder))]
    public List<PlaceTranslationRequest> PlaceTranslation { get; set; }

}

