﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.ViewModel
{
    public class TagRequest
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }
}