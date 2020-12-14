using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public partial class PromotionAnalytics
    {
        public string PromotionId { get; set; }
        public string AdvertismentId { get; set; }
        public string InstitutionId { get; set; }
        public int? Count { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Type { get; set; }
    }

    public class AnalyticsModel
    {
        public List<PromotionAnalytics> analytics { get; set; }
    }
}
