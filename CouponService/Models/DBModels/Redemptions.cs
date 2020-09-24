using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Redemptions
    {
        public int RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CouponId { get; set; }
        public int? OfficerId { get; set; }

        public virtual Coupons Coupon { get; set; }
    }
}
