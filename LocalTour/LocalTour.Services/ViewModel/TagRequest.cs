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
    public class TagRequest
    {

        public string TagName { get; set; } = null!;
        public string TagVi { get; set; } = null!;

        public IFormFile TagPhotoUrl { get; set; } = null!;
    }
}
