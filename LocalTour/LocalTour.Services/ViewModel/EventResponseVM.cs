using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;

namespace LocalTour.Services.ViewModel;

public class EventResponseVM : IMapFrom<Event>
{
    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    public string? EventPhotoDisplay { get; set; }
}