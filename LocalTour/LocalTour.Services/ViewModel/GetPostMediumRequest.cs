using LocalTour.Services.ViewModel;

public class GetPostMediumRequest : PaginatedQueryParams
{
    public int? PostId { get; set; }
    public string? Type { get; set; } // Optional filter to differentiate Photo/Video
}
