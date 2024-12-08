using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class BannerHistory
{
    public Guid Id { get; set; }
    public Guid BannerId { get; set; }
    public DateTime TimeStart { get; set; }
    public DateTime TimeEnd { get; set; }
    public string Status { get; set; }
    public Guid ApproverId { get; set; }
    [JsonIgnore]
    public Banner Banner { get; set; }
    [JsonIgnore]
    public User Approver { get; set; }
}