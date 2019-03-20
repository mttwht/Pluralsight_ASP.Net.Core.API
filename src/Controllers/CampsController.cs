using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks=false)
        {
            try {
                var result = await campRepository.GetAllCampsAsync(includeTalks);

                return mapper.Map<CampModel[]>(result);
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try {
                var result = await campRepository.GetCampAsync(moniker);

                if(result == null)
                    return NotFound();

                return mapper.Map<CampModel>(result);
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks=false)
        {
            try {
                var result = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if(!result.Any())
                    return NotFound();

                return mapper.Map<CampModel[]>(result);
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
    }
}