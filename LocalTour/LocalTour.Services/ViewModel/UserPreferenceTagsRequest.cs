using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class UserPreferenceTagsRequest
    {
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
