using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetDestinationRequest : PaginatedQueryParams
    {
        public string? FilterBy { get; set; }
        public string? LanguageCode { get; set; }
    }
}
