namespace LocalTour.Services.ViewModel;

public class ChangePasswordRequest
{
    public string oldPassword  {get; set;}
    public string newPassword {get; set;}
    public string confirmPassword {get; set;}
}