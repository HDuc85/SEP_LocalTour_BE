using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceReportRequest
    {
        public int?Id { get; set; }
        public Guid UserReportId { get; set; }
        public int PlaceId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
