using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Places
    {
        public Places()
        {
            PromotionsPlaces = new HashSet<PromotionsPlaces>();
        }

        public int PlaceId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PromotionsPlaces> PromotionsPlaces { get; set; }
    }
}
