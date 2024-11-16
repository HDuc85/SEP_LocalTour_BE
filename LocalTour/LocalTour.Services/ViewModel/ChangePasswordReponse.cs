namespace LocalTour.Services.ViewModel;

public class ChangePasswordReponse
{
    public bool Success { get; set; }
    public string OldPasswordError { get; set; }
    public string NewPasswordError { get; set; }
}