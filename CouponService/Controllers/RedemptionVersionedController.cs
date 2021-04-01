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
    public class RedemptionVersionedController : ControllerBase
    {
        private readonly IRedemptionRepository _redemptionRepository;
        public RedemptionVersionedController(IRedemptionRepository redemptionRepository)
        {
            _redemptionRepository = redemptionRepository;
        }

        [HttpPost]
        [Route("coupons/redeem")]
        public IActionResult Post(RedemptionModel model)
        {
            dynamic response = _redemptionRepository.InsertRedemption(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("coupons/redeem/{id=0}")]
        public IActionResult Get(string id, string officerId, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _redemptionRepository.GetRedemption(id, officerId, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("coupons/redeem/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _redemptionRepository.DeleteRedemption(id);
            return StatusCode((int)response.statusCode, response);
        }


        [HttpGet]
        [Route("coupons/redeem/search")]
        public IActionResult GetSearchRedemption(string officerId, string q, string cxt, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _redemptionRepository.SearchRedemption(officerId, q, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
