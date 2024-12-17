using Microsoft.AspNetCore.Http;

namespace LocalTour.Services.ViewModel;

public class CreateModCheckRequest
{
    public int PlaceId { get; set; }
    public List<IFormFile> Files { get; set; }
}