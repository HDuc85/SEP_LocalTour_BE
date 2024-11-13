using Azure.Core;
using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
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
    public class PlaceActivityRequest : IMapFrom<PlaceActivity>
    {
        public int DisplayNumber { get; set; }

        public IFormFile? PhotoDisplay { get; set; }
        public List<IFormFile> PlaceActivityMedium { get; set; }
        [ModelBinder(BinderType = typeof(FromJsonBinder))]
        public List<PlaceActivityTranslationRequest> PlaceActivityTranslations { get; set; }

    }
}
