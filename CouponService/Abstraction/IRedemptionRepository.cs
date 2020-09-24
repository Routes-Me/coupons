using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IRedemptionRepository
    {
        dynamic GetRedemption(string id, Pagination pageInfo, string includedType);
        dynamic UpdateRedemption(RedemptionModel model);
        dynamic DeleteRedemption(string id);
        dynamic InsertRedemption(RedemptionModel model);
    }
}
