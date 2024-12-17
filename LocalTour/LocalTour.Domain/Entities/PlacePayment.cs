namespace LocalTour.Domain.Entities;

public class PlacePayment
{
    public long Id { get; set; }
    
    public long OrderCode { get; set; }
    
    public int PlaceId { get; set; }
    
    public string Status { get; set; }
}