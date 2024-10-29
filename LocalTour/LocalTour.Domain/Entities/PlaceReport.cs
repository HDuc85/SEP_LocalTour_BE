using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceReport
{
    public int Id { get; set; }
    
    public Guid UserReportId { get; set; }

    public int PlaceId { get; set; }

    public DateTime ReportDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public virtual Place Place { get; set; } = null!;
}
