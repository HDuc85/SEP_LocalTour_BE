using LocalTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceTagRequest
    {
        public int Id { get; set; }

        public int TagId { get; set; }

        public int PlaceId { get; set; }

        public virtual Tag Tag { get; set; } = null!;
    }
}
