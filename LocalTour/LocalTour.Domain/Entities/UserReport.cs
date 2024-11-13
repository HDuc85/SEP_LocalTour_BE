using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class UserReport
{
    public int Id { get; set; }
    public Guid UserReportId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = null!;
    public string Content { get; set; } = null!;
    
    public virtual User UserReport1 { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
