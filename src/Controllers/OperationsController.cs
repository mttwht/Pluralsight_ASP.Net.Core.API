using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public OperationsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("reloadconfig")]
        public ActionResult ReloadConfig()
        {
            try {
                var configRoot = (IConfigurationRoot)configuration;
                configRoot.Reload();
                return Ok();
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
