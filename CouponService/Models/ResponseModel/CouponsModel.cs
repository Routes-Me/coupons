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

    public class CouponsModelForIncludeData 
    {
        public string CouponId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string LogoUrl { get; set; }
        public DateTime? ExpieryDate { get; set; }
    }
}
