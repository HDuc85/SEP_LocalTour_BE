using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceReportViewModel : PaginatedQueryParams
    {
        public string? Status { get; set; }
        public List<int>? DistrictNCityIds { get; set; }
    }
}
