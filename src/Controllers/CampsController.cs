using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
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

        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try {
                var link = linkGenerator.GetPathByAction(nameof(Get), "Camps", new { moniker = model.Moniker });

                if(string.IsNullOrEmpty(link))
                    return BadRequest();

                var existing = await campRepository.GetCampAsync(model.Moniker);
                if(existing != null)
                    return BadRequest("Moniker already exists");

                var camp = mapper.Map<Camp>(model);
                campRepository.Add(camp);
                if(await campRepository.SaveChangesAsync()) {
                    return Created(link, mapper.Map<CampModel>(camp));
                }
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try {
                var camp = await campRepository.GetCampAsync(moniker);
                if(camp == null)
                    return NotFound($"Camp with moniker {moniker} not found");

                mapper.Map(model, camp);

                if(await campRepository.SaveChangesAsync()) {
                    return mapper.Map<CampModel>(camp);
                }
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try {
                var camp = await campRepository.GetCampAsync(moniker);

                if(camp == null)
                    return NotFound();

                campRepository.Delete(camp);

                if(await campRepository.SaveChangesAsync())
                    return Ok();
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }
    }
}