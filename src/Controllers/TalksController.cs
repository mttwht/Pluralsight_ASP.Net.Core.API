using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker, true);
                return mapper.Map<TalkModel[]>(talks);
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id, true);
                return mapper.Map<TalkModel>(talk);
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try {
                var camp = await campRepository.GetCampAsync(moniker);

                var talk = mapper.Map<Talk>(model);

                if(camp == null)
                    return BadRequest($"Camp {moniker} not found");
                talk.Camp = camp;

                if(model.Speaker == null)
                    return BadRequest("Speaker is required");
                var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if(speaker == null)
                    return BadRequest($"Speaker {model.Speaker.SpeakerId} not found");
                talk.Speaker = speaker;

                campRepository.Add(talk);

                if(await campRepository.SaveChangesAsync()) {
                    var link = linkGenerator.GetPathByAction(HttpContext,
                        nameof(Get),
                        values: new { moniker, id = talk.TalkId });

                    return Created(link, mapper.Map<TalkModel>(talk));
                }
                else {
                    return BadRequest("Could not save talk");
                }
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id, true);

                if(talk == null)
                    return BadRequest($"Talk {id} not found");

                mapper.Map(model, talk);

                if(model.Speaker != null) {
                    var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if(speaker != null)
                        talk.Speaker = speaker;
                }

                if(await campRepository.SaveChangesAsync()) {
                    return mapper.Map<TalkModel>(talk);
                }
                else {
                    return BadRequest("Could not update talk");
                }
            }
            catch(Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
