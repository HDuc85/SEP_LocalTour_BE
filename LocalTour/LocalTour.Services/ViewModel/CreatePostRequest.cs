using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LocalTour.Services.ViewModel
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public int? PlaceId { get; set; }
        public int? ScheduleId { get; set; }
        public bool Public { get; set; }

        // Thêm thuộc tính MediaFiles để chứa tệp ảnh hoặc video
        public List<IFormFile> MediaFiles { get; set; } = new List<IFormFile>();
    }
}
