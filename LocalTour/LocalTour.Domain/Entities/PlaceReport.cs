﻿using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class PlaceReport
{
    public int Id { get; set; }
    public Guid UserReportId { get; set; }
    public int PlaceId { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = null!;
    
    public string Content { get; set; } = null!;

    public virtual User UserReport { get; set; } = null!;
    public virtual Place Place { get; set; } = null!;
}
