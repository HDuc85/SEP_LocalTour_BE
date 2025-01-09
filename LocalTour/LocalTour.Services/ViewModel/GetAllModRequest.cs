namespace LocalTour.Services.ViewModel;

public class GetAllModRequest : PaginatedQueryParams
{
    public int? DistricNCityId { get; set; }
    public int? PlaceId { get; set; }
}