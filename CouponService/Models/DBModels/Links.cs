using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Links
    {
        public int LinkId { get; set; }
        public int? PromotionId { get; set; }
        public string Web { get; set; }
        public string Ios { get; set; }
        public string Android { get; set; }

        public virtual Promotions Promotion { get; set; }
    }
}
