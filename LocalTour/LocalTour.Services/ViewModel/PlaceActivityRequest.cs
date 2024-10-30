using Azure.Core;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceActivityRequest : IMapFrom<PlaceActivity>
    {
        public int DisplayNumber { get; set; }

        public string? PhotoDisplay { get; set; }
        public List<IFormFile> PlaceActivityMedium { get; set; }
        public List<PlaceActivityTranslationRequest> PlaceActivityTranslations { get; set; }

    }
}
