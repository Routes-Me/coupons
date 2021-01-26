using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IRedemptionRepository
    {
        dynamic GetRedemption(string redemptionId, string officerId, Pagination pageInfo, string includedType);
        dynamic DeleteRedemption(string id);
        dynamic InsertRedemption(RedemptionModel model);
        dynamic SearchRedemption(string officerId, string q, Pagination pageInfo, string includedType);
    }
}
