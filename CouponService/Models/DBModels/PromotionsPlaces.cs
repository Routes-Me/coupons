using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class PromotionsPlaces
    {
        public int PromotionId { get; set; }
        public int PlaceId { get; set; }

        public virtual Places Place { get; set; }
        public virtual Promotions Promotion { get; set; }
    }
}
