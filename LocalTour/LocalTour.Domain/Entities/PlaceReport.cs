using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalTour.Domain.Entities;

public partial class PlaceReport
{
    public int Id { get; set; }
    public Guid UserReportId { get; set; }
    public int PlaceId { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = null!;
    
    public string Content { get; set; } = null!;

    [JsonIgnore]
    public virtual User UserReport { get; set; } = null!;
    [JsonIgnore]
    public virtual Place Place { get; set; } = null!;
}
