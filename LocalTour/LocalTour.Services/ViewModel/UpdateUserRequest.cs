using Microsoft.AspNetCore.Http;

namespace LocalTour.Services.ViewModel
{
    public class UpdateUserRequest
    {
        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
