namespace LocalTour.Domain.Entities;

public class UpdateScheduleRequest
{
    public int ScheduleId { get; set; }
    public string ScheduleName { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsPublic { get; set; }
}