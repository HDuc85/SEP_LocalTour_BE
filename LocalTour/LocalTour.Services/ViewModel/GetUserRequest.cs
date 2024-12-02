using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetUserRequest
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Gender { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public List<string> Roles { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set;}
    }
}
