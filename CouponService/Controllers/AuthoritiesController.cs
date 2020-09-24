using System.Threading.Tasks;
using CouponService.Abstraction;
using CouponService.Models;
using CouponService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CouponService.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthoritiesController : ControllerBase
    {
        private readonly IAuthoritiesRepository _authoritiesRepository;
        public AuthoritiesController(IAuthoritiesRepository authoritiesRepository)
        {
            _authoritiesRepository = authoritiesRepository;
        }

        [HttpPost]
        [Route("authorities")]
        public IActionResult Post(AuthoritiesModel model)
        {
            dynamic response = _authoritiesRepository.InsertAuthorities(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet]
        [Route("authorities/{id=0}")]
        public IActionResult Get(string id, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _authoritiesRepository.GetAuthorities(id, pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPut]
        [Route("authorities")]
        public IActionResult Put(AuthoritiesModel model)
        {
            dynamic response = _authoritiesRepository.UpdateAuthorities(model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("authorities/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _authoritiesRepository.DeleteAuthorities(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
