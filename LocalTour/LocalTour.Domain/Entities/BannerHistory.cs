namespace LocalTour.Domain.Entities;

public partial class BannerHistory
{
    public Guid Id { get; set; }
    public Guid BannerId { get; set; }
    public DateTime TimeStart { get; set; }
    public DateTime TimeEnd { get; set; }
    public string Status { get; set; }
    public Guid ApproverId { get; set; }

    public Banner Banner { get; set; }
    public User Approver { get; set; }
}