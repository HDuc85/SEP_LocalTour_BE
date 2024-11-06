using LocalTour.Domain.Entities;
using LocalTour.Services.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class UserPreferenceTagsRequest : IMapFrom<UserPreferenceTags>
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public Guid UserId { get; set; }
    }
}
