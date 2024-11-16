using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class UpdateTagRequest
    {
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
