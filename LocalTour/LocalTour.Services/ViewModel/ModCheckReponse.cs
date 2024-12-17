using LocalTour.Domain.Entities;

namespace LocalTour.Services.ViewModel;

public class ModCheckReponse
{
    public Guid ModId { get; set; }
    public string? ModName { get; set; }
    public int PlaceId { get; set; }
    public List<PlaceTranslation>? PlaceTranslations { get; set; }
    public List<PlaceMedium>? PlaceMediums { get; set; }
    public List<string>? ModeCheckImages { get; set; }
}