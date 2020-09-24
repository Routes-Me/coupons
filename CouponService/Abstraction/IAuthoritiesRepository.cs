using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IAuthoritiesRepository
    {
        dynamic GetAuthorities(string id, Pagination pageInfo, string includeType);
        dynamic UpdateAuthorities(AuthoritiesModel model);
        dynamic DeleteAuthorities(string id);
        dynamic InsertAuthorities(AuthoritiesModel model);
    }
}
