using LocalTour.Domain.Entities;

namespace LocalTour.Services.ViewModel;

public class BannerRespone
{
    public Guid Id { get; set; }
    public string BannerName { get; set; }
    public string BannerUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<BannerHistoryResponse> BannerHistories { get; set; } = new List<BannerHistoryResponse>();

}

public class BannerHistoryResponse
{
    public Guid Id { get; set; }
    public Guid BannerId { get; set; }
    public DateTime TimeStart { get; set; }
    public DateTime TimeEnd { get; set; }
    public string Status { get; set; }
}