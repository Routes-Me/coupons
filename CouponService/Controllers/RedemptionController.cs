using System.Collections.Generic;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [Route("api")]
    [ApiController]
    public class RedemptionController : ControllerBase
    {
        private readonly IRedemptionRepository _redemptionRepository;
        public RedemptionController(IRedemptionRepository redemptionRepository)
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
        [Route("coupons/redeem/{redemptionId?}")]
        public IActionResult Get(string redemptionId, string officerId, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _redemptionRepository.GetRedemption(redemptionId, officerId, pageInfo, Include);
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
