namespace LocalTour.Services.ViewModel;

public class BannerHistoryRequest
{
    public Guid BannerId { get; set; }
    public DateTime TimeStart { get; set; }
    public DateTime TimeEnd { get; set; }
}