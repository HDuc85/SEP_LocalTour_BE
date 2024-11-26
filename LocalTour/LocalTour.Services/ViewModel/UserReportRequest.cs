using System;

namespace LocalTour.Services.ViewModel
{
        public class UserReportRequest
        {
                public Guid UserId { get; set; }
                public string Content { get; set; } = null!;
                public DateTime ReportDate { get; set; }
                public string Status { get; set; } = null!;
        }
}
