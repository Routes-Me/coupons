using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface ICouponsRepository
    {
        dynamic GetCoupons(string couponId, string userId, string promotionsId, Pagination pageInfo, string includedType);
        dynamic DeleteCoupons(string id);
        dynamic InsertCoupons(CouponsModel model);
    }
}
