using LocalTour.Services.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceUpdateRequest
    {
        public int WardId { get; set; }

        public IFormFile PhotoDisplay { get; set; } = null!;

        public TimeOnly TimeOpen { get; set; }

        public TimeOnly TimeClose { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }
        public string ContactLink { get; set; }
        public List<int> Tags { get; set; }
        public List<string>? PlaceMedia { get; set; }
        [ModelBinder(BinderType = typeof(FromJsonBinder))]
        public List<PlaceTranslationRequest> PlaceTranslation { get; set; }
    }
}
