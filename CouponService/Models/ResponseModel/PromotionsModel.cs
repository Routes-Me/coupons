using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Models.ResponseModel
{
    public class PromotionsPostModel
    {
        public string PromotionId { get; set; }
        public string PlaceId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string QrCodeUrl { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime? ExpieryDate { get; set; }
        public string AdvertisementId { get; set; }
        public string InstitutionId { get; set; }
        public bool? IsSharable { get; set; }
        public string LogoUrl { get; set; }
        public string Type { get; set; }
        public LinkPostModel Links { get; set; }

    }

    public class PromotionsModel
    {
        public string PromotionId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int? UsageLimit { get; set; }
        public string AdvertisementId { get; set; }
        public string InstitutionId { get; set; }
        public bool? IsSharable { get; set; }
        public string LogoUrl { get; set; }
        public string Type { get; set; }
    }

    public class LinkPostModel
    {
        public string Web { get; set; }
        public string Ios { get; set; }
        public string Android { get; set; }
    }
}
