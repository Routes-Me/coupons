using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class CouponsModel
    {
        public string CouponId { get; set; }
        public string PromotionId { get; set; }
        public string UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
