using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class UpdatePostRequest
    {
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public int? PlaceId { get; set; }
        public int? ScheduleId { get; set; }
        public bool Public { get; set; }

        // Thêm thuộc tính MediaFiles để chứa tệp ảnh hoặc video
        public List<IFormFile> MediaFiles { get; set; } = new List<IFormFile>();
    }
}
