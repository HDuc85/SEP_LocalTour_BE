namespace LocalTour.Services.ViewModel;

public class SendMailRequest
{
    public int PlaceId { get; set; }
    public string? RejectReason { get; set; }
    public bool IsApproved { get; set; }
}