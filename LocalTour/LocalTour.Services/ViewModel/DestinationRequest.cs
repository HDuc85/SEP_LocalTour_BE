﻿using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class DestinationRequest : IMapFrom<Destination>
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int PlaceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Detail { get; set; }
        public bool IsArrived { get; set; }
        public string? PlacePhotoDisplay { get; set; }
        public string? PlaceName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
