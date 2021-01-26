using CouponService.Models;
using CouponService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponService.Abstraction
{
    public interface ILinksRepository
    {
        dynamic InsertLinks(LinksModel model);
        dynamic GetLinks(string linkId, string promotionId, Pagination pageInfo, string includeType);
        dynamic UpdateLinks(LinksModel model);
        dynamic DeleteLinks(string id);
    }
}
