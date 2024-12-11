using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel;

public class GetEventRequest : PaginatedQueryParams
{
    public string languageCode { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string? status { get; set; }
}
