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
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public String? ProfileUrl { get; set; }
        public String? UserName { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
        public int? TotalLike { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool isLiked { get; set; }
        public virtual ICollection<PlaceFeeedbackMedium> PlaceFeeedbackMedia { get; set; } = new List<PlaceFeeedbackMedium>();
    }
}
