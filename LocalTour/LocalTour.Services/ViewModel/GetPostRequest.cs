using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetPostRequest : PaginatedQueryParams
    {
        public string? FilterBy { get; set; }  // Optional: filter by PlaceId, ScheduleId, etc.
    }
}
