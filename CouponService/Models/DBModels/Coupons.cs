using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Coupons
    {
        public Coupons()
        {
            Redemptions = new HashSet<Redemptions>();
        }

        public int CouponId { get; set; }
        public int? PromotionId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Promotions Promotion { get; set; }
        public virtual ICollection<Redemptions> Redemptions { get; set; }
    }
}
