using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Abstract
{
    public interface IPlaceTranslationService
    {
        Task<int> Create(PlaceTranslation placeTranslation);
    }
}
