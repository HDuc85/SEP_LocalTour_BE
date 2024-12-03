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
    public class TagViewModel : IMapFrom<Tag>
    {
        public int Id { get; set; }
        public string TagName { get; set; } = null!;
        public string TagVi { get; set; } = null!;
        public string TagPhotoUrl { get; set; } = null!;
    }
}
