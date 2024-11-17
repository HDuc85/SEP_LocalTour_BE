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
        public List<TagVM> Tags { get; set; } = new List<TagVM>();
        public Guid UserId { get; set; }
    }
}
