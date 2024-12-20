namespace LocalTour.Services.ViewModel;
public class PaginatedQueryParams
{
    public int? Page { get; set; }
    public int? Size { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}