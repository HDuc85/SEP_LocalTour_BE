using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class Banner
{
    public Guid Id { get; set; }
    public string BannerName { get; set; }
    public string BannerUrl { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public User Author { get; set; }
    public ICollection<BannerHistory> BannerHistories { get; set; } = new List<BannerHistory>();
}