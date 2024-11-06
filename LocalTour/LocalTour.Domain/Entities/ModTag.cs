namespace LocalTour.Domain.Entities;

public partial class ModTag
{
    public Guid UserId { get; set; }
    
    public int TagId { get; set; }
    
    public virtual User User { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}