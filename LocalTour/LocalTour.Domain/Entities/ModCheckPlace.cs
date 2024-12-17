namespace LocalTour.Domain.Entities;

public class ModCheckPlace
{
    public long Id { get; set; }
    
    public Guid ModId { get; set; }
    
    public int PlaceId { get; set; }
    
    public string ImageUrl { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public virtual Place Place { get; set; } = null!;
        
    public virtual User Mod { get; set; } = null!;
}