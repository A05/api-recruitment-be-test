using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiApplication.Models;
using AutoMapper;
using ApiApplication.Services;
using ApiApplication.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimeController : ControllerBase
    {
        private readonly ICinemaService _service;
        private readonly IMapper _mapper;

        public ShowtimeController(ICinemaService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Policy = "Read")]
        public async Task<ActionResult<IEnumerable<ShowtimeModel>>> Get()
        {
            var entities = await _service.GetAsync();
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Read")]
        public async Task<ActionResult<ShowtimeModel>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            var model = _mapper.Map<ShowtimeModel>(entity);

            return Ok(model);
        }

        [HttpGet("date/{date}")]
        [Authorize(Policy = "Read")]
        public async Task<ActionResult<IEnumerable<ShowtimeModel>>> GetByDate(DateTime date)
        {
            var entities = await _service.GetByDateAsync(date);
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpGet("movie/{title}")]
        [Authorize(Policy = "Read")]
        public async Task<ActionResult<ShowtimeModel>> GetByTitle(string title)
        {
            var entity = await _service.GetByTitleAsync(title);
            if (entity == null)
                return NotFound();

            var model = _mapper.Map<ShowtimeModel>(entity);

            return Ok(model);
        }

        [HttpPost]
        [Authorize(Policy = "Write")]
        public async Task<ActionResult<ShowtimeModel>> Create(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);

            var (success, createdEntity) = await _service.TryCreateAsync(entity);

            if (!success)
            {
                Response.Headers.Location = Url.Action(nameof(GetById), new { id = createdEntity.Id });
                
                return Conflict($"The movie with IMDB ID {model.Movie.ImdbId} already exists.");
            }
            
            var createdModel = _mapper.Map<ShowtimeModel>(createdEntity);

            return CreatedAtAction(nameof(GetById), new { createdModel.Id }, createdModel);
        }

        [HttpPut]
        [Authorize(Policy = "Write")]
        public async Task<ActionResult<ShowtimeModel>> Update(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);

            var (success, updatedEntity) =await _service.TryUpdateAsync(entity);

            if (!success)
                return NotFound();
            
            var updatedModel = _mapper.Map<ShowtimeModel>(updatedEntity);

            return Ok(updatedModel);
        }

        [HttpPatch]
        [Authorize(Policy = "Write")]
        public IActionResult Update()
        {
            // patch -h Content-Type=application/json --no-body

            throw new ApplicationException("I am a terrible error! Catch me outside of this class if you can.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Write")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.TryDeleteAsync(id))
                return NotFound();

            return NoContent();
        }
    }
}
