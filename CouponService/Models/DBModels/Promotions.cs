using System;
using System.Collections.Generic;

namespace CouponService.Models.DBModels
{
    public partial class Promotions
    {
        public Promotions()
        {
            Coupons = new HashSet<Coupons>();
            PromotionsPlaces = new HashSet<PromotionsPlaces>();
        }

        public int PromotionId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string QrCodeUrl { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime? ExpieryDate { get; set; }
        public int? AdvertisementId { get; set; }
        public int? InstitutionId { get; set; }
        public bool? IsSharable { get; set; }
        public string LogoUrl { get; set; }

        public virtual ICollection<Coupons> Coupons { get; set; }
        public virtual ICollection<PromotionsPlaces> PromotionsPlaces { get; set; }
    }
}
