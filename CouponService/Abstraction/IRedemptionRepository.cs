﻿using CouponService.Models;
using CouponService.Models.ResponseModel;

namespace CouponService.Abstraction
{
    public interface IRedemptionRepository
    {
        dynamic GetRedemption(string id, string officerId, Pagination pageInfo, string includedType);
        dynamic DeleteRedemption(string id);
        dynamic InsertRedemption(RedemptionModel model);
    }
}