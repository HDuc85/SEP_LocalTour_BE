using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Http;

public class GetPostMediumRequest : PaginatedQueryParams
{
    public string Type { get; set; } = null!;  // 'photo' or 'video'
    public IFormFile File { get; set; } = null!;
}
