using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceActivityTranslationRequest
    {
        public string LanguageCode { get; set; } = null!;

        public string ActivityName { get; set; } = null!;

        public double Price { get; set; }

        public string? Description { get; set; }

        public string PriceType { get; set; } = null!;

        public double? Discount { get; set; }
    }
}
