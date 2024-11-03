using System.ComponentModel.DataAnnotations;

namespace LocalTour.Services.ViewModel;

public class CreateUserRequest 
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
}