using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [Route("api")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        private readonly ILinksRepository _linksRepository;
        public LinksController(ILinksRepository linksRepository)
        {
            _linksRepository = linksRepository;
        }

        [HttpPost]
        [Route("links")]
        public IActionResult Post(LinksModel model)
        {
            dynamic response = _linksRepository.InsertLinks(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("links/{linkId?}")]
        public IActionResult Get(string linkId, string promotionId, string include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _linksRepository.GetLinks(linkId, promotionId, pageInfo, include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPut]
        [Route("links")]
        public IActionResult Put(LinksModel model)
        {
            dynamic response = _linksRepository.UpdateLinks(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("links/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _linksRepository.DeleteLinks(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
