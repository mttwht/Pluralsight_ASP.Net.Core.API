using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : ControllerBase
    {
        public object Get()
        {
            return new { Moniker = "DUB2019", Name = "Dublin Code Camp" };
        }
    }
}