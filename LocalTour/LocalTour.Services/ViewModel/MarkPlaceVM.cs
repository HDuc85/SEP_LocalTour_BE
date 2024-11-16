namespace LocalTour.Services.ViewModel;

public class MarkPlaceVM
{
    public int Id { get; set; }
    //public Guid UserId { get; set; }

    public int PlaceId { get; set; }

    public string PlaceName { get; set; }
    
    public string PhotoDisplay { get; set; }
    
    public bool IsVisited { get; set; }
    
    //public int VisitTime { get; set; }
    public DateTime createdDate { get; set; }
}
