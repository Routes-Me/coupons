﻿using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface ICouponsRepository
    {
        dynamic GetCoupons(string id, Pagination pageInfo, string includedType);
        dynamic DeleteCoupons(string id);
        dynamic InsertCoupons(CouponsModel model);
    }
}
