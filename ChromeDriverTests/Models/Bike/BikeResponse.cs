﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDriverTests.Models.Bike
{
    public class BikeResponse
    {
        public string Id { get; set; }
        public StationResponse Station { get; set; }
        public string Status { get; set; }
    }
}
