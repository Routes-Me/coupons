using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class PlacesModel
    {
        public string PlaceId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Name { get; set; }
    }
}
