using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageModels.Models.Backend
{
    public class Malfunction
    {
        public string Id { get; set; }
        public string BikeId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
    }
}
