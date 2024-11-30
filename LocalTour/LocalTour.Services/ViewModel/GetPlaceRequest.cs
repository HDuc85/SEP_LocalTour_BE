using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel;
public class GetPlaceRequest : PaginatedQueryParams
{
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public string LanguageCode { get; set; }
    public List<int>? Tags { get; set; }
    
    public string? Status { get; set; }
}

