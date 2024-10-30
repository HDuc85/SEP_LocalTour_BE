namespace LocalTour.Domain.Entities;

public partial class UserPreferenceTags
{
    public int Id { get; set; }
    
    public int TagId { get; set; }
    
    public Guid UserId { get; set; }
    
    public virtual Tag Tag { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
}