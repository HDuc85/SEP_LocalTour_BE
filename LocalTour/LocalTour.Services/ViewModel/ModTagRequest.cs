using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class ModTagRequest
    {
        public Guid UserId { get; set; }
        public int TagId { get; set; }
    }
}
