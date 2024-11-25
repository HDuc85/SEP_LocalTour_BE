using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetScheduleRequest : PaginatedQueryParams
    {
        public Guid? UserId { get; set; }
        public String languageCode { get; set; }
    }
}