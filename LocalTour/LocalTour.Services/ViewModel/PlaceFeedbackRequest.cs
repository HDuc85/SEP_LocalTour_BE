using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class PlaceFeedbackRequest : IMapFrom<PlaceFeeedback>
    {
        public int Rating { get; set; }
        public string? Content { get; set; }
        public int? TotalLike { get; set; }
        public bool isLiked { get; set; }
        public virtual ICollection<PlaceFeeedbackMedium> PlaceFeeedbackMedia { get; set; } = new List<PlaceFeeedbackMedium>();
    }
}
