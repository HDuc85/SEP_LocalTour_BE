namespace LocalTour.Domain.Entities;

public partial class ModTag
{
    public Guid UserId { get; set; }
    
    public int DistrictNcityId { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual DistrictNcity DistrictNcity { get; set; } = null!;
}