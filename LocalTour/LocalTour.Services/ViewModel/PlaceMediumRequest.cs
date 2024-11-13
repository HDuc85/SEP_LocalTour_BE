using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceMediumRequest
    {
        public int Id { get; set; }

        public int PlaceId { get; set; }

        public string Type { get; set; } = null!;

        public string Url { get; set; } = null!;

        public DateTime CreateDate { get; set; }
    }
}
