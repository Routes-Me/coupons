using System;

namespace CouponService.Models.ResponseModel
{
    public class RedemptionGetModel
    {
        public string RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
    }

    public class RedemptionModel
    {
        public string RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
        public string Pin { get; set; }
        public string InstitutionId { get; set; }
    }
}
