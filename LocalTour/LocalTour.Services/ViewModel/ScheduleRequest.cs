using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class ScheduleRequest : IMapFrom<Schedule>
    {
        public int? Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImage { get; set; }
        public string ScheduleName { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } = null!;
        public bool? IsPublic { get; set; }
        public List<DestinationRequest> Destinations { get; set; }
        public int TotalLikes { get; set; }
        public bool IsLiked { get; set; }
    }
}
