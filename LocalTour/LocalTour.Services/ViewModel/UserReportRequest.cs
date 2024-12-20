using System;

namespace LocalTour.Services.ViewModel
{
        public class UserReportRequest
        {
        public Guid UserId { get; set; }
        public string? Content { get; set; } = null!;
        }

        public class ChangeStatus
        {
                public int Id { get; set; }
                public string? Status { get; set; }
        }
}
