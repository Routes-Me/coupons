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
        [Route("coupons/redeem/{id=0}")]
        public IActionResult Get(string id, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _redemptionRepository.GetRedemption(id, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPut]
        [Route("coupons/redeem")]
        public IActionResult Put(RedemptionModel model)
        {
            dynamic response = _redemptionRepository.UpdateRedemption(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("coupons/redeem/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _redemptionRepository.DeleteRedemption(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
