using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;

        public CampsController(ICampRepository campRepository)
        {
            this.campRepository = campRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try {
                var result = await campRepository.GetAllCampsAsync();

                return Ok(result);
            }
            catch(System.Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
    }
}