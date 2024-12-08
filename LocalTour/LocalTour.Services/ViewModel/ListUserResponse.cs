namespace LocalTour.Services.ViewModel;

public class ListUserResponse
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? FullName { get; set; }
}