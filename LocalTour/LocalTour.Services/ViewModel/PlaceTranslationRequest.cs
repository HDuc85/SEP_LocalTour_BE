namespace LocalTour.Services.ViewModel
{
    public class PlaceTranslationRequest
    {
        public string LanguageCode { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Address { get; set; } = null!;

        public string? Contact { get; set; }
    }
}
