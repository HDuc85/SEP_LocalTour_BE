using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class GetUserPreferenceTagsRequest
    {
        public int Id { get; set; }
        public List<TagRequest> Tags { get; set; } = new List<TagRequest>();
        public Guid UserId { get; set; }
    }
}
