using System.Collections.Generic;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponsRepository _couponsRepository;
        public CouponsController(ICouponsRepository couponsRepository)
        {
            _couponsRepository = couponsRepository;
        }

        [HttpPost]
        [Route("coupons")]
        public IActionResult Post(CouponsModel model)
        {
            dynamic response = _couponsRepository.InsertCoupons(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("coupons/{couponId?}")]
        public IActionResult Get(string couponId, string userId, string promotionId, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _couponsRepository.GetCoupons(couponId, userId, promotionId, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("coupons/{id}")]
        public IActionResult Delete(string id)  
        {
            dynamic response = _couponsRepository.DeleteCoupons(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
