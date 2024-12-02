namespace LocalTour.Services.ViewModel;

public class UserProfileVM
{
    
    public string? fullName { get; set; }
    public string? userName { get; set; }
    public string? userProfileImage { get; set; }
    public string? email { get; set; }
    public DateTime? dateOfBirth { get; set; }
    public string? gender { get; set; }
    public string? address { get; set; }
    public string? phoneNumber { get; set; }
    public int? totalSchedules { get; set; }
    public int? totalPosteds { get; set; }
    public int? totalReviews { get; set; }
    public int? totalFollowed { get; set; }
    public int? totalFollowers { get; set; }
    public bool? isFollowed { get; set; }
    public bool? isHasPassword { get; set; }
    public List<string> roles { get; set; }
}