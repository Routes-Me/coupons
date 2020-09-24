using System.Collections.Generic;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [Route("api")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly IPlacesRepository _placesRepository;
        public PlacesController(IPlacesRepository placesRepository)
        {
            _placesRepository = placesRepository;
        }

        [HttpPost]
        [Route("places")]
        public IActionResult Post(PlacesModel model)
        {
            dynamic response = _placesRepository.InsertPlaces(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("places/{id=0}")]
        public IActionResult Get(string id, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _placesRepository.GetPlaces(id, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPut]
        [Route("places")]
        public IActionResult Put(PlacesModel model)
        {
            dynamic response = _placesRepository.UpdatePlaces(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("places/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _placesRepository.DeletePlaces(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
