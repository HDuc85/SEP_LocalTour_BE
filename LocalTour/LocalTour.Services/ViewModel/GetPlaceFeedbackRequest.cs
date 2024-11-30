using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetPlaceFeedbackRequest : PaginatedQueryParams
    {
        public int? PlaceId { get; set; }
        public Guid? UserId { get; set; }
        public string? LanguageCode { get; set; }
    }
}
