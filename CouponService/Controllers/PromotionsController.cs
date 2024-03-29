﻿using System.Collections.Generic;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionsRepository _promotionsRepository;
        public PromotionsController(IPromotionsRepository promotionsRepository)
        {
            _promotionsRepository = promotionsRepository;
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult Post(PromotionsPostModel model)
        {
            dynamic response = _promotionsRepository.InsertPromotions(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("promotions/{promotionId?}")]
        public IActionResult Get(string promotionId, string advertisementId, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _promotionsRepository.GetPromotions(promotionId, advertisementId, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("promotions/contents/{advertisementId?}")]
        public IActionResult GetPromotions(string advertisementId, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _promotionsRepository.GetPromotionsByAdvertisementsId(advertisementId, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPut]
        [Route("promotions")]
        public IActionResult Put(PromotionsPostModel model)
        {
            dynamic response = _promotionsRepository.UpdatePromotions(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("promotions/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _promotionsRepository.DeletePromotions(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
