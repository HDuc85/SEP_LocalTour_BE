namespace LocalTour.Services.ViewModel;

public class SendEmailModel
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string ServiceOwnerName { get; set; }
    public string PlaceName { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }
}