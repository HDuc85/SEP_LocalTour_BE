namespace LocalTour.Services.ViewModel;

public class ChangePasswordResponse
{
    public bool Success { get; set; }
    public string OldPasswordError { get; set; }
    public string NewPasswordError { get; set; }
}