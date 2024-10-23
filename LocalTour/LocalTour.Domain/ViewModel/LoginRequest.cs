using System.ComponentModel.DataAnnotations;

namespace LocalTour.Domain.ViewModel
{
    public class LoginRequest
    {
        [Phone]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
