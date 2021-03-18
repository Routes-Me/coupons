using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IPromotionsRepository
    {
        dynamic GetPromotions(string promotionId, string advertisementId, Pagination pageInfo, string includedType);
        dynamic UpdatePromotions(PromotionsPostModel model);
        dynamic DeletePromotions(string id);
        dynamic InsertPromotions(PromotionsPostModel model);
        dynamic GetPromotionsByAdvertisementsId(string advertisementId, Pagination pageInfo);
    }
}
