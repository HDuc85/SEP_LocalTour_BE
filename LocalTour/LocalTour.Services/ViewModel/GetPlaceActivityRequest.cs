﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel;
    public class GetPlaceActivityRequest : PaginatedQueryParams
{
    public int PlaceId { get; set; }
}
