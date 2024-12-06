using Microsoft.AspNetCore.Http;

namespace LocalTour.Services.ViewModel;

public class BannerRequest
{
    public string BannerName { get; set; }
    public IFormFile BannerUrl { get; set; }
}