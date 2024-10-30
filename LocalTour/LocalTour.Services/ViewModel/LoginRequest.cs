using System.ComponentModel.DataAnnotations;

namespace LocalTour.Services.ViewModel
{
    public class LoginRequest
    {
        [Phone]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
