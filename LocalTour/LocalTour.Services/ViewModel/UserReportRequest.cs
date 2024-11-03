using System;

namespace LocalTour.Services.ViewModel
{
    public class UserReportRequest
    {
        public int Id { get; set; }
        public Guid UserReportId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
