using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiApplication.Models;
using AutoMapper;
using ApiApplication.Services;
using ApiApplication.Database.Entities;
using Microsoft.AspNetCore.Http;

namespace ApiApplication.Controllers
{
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
        public ActionResult<IEnumerable<ShowtimeModel>> Get()
        {
            var entities = _service.Get();
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpGet("{id}")]
        public ActionResult<ShowtimeModel> GetById(int id)
        {
            var entity = _service.GetById(id);
            if (entity == null)
                return NotFound();

            var model = _mapper.Map<ShowtimeModel>(entity);

            return Ok(model);
        }

        [HttpGet("date/{date}")]
        public ActionResult<IEnumerable<ShowtimeModel>> GetByDate(DateTime date)
        {
            var entities = _service.GetByDate(date);
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpGet("movie/{title}")]
        public ActionResult<ShowtimeModel> GetByTitle(string title)
        {
            var entity = _service.GetByTitle(title);
            if (entity == null)
                return NotFound();

            var model = _mapper.Map<ShowtimeModel>(entity);

            return Ok(model);
        }

        [HttpPost]
        public ActionResult<ShowtimeModel> Create(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);

            if (!_service.TryCreate(entity, out var createdEntity))
            {
                Response.Headers.Location = Url.Action(nameof(GetById), new { id = createdEntity.Id });
                
                return Conflict($"The movie with IMDB ID {model.Movie.ImdbId} already exists.");
            }
            
            var createdModel = _mapper.Map<ShowtimeModel>(createdEntity);

            return CreatedAtAction(nameof(GetById), new { createdModel.Id }, createdModel);
        }

        [HttpPut]
        public IActionResult Update(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);
            var updatedEntity = _service.Update(entity);
            var updatedModel = _mapper.Map<ShowtimeModel>(updatedEntity);

            // TODO: Return updatedModel. Figure out the proper HTTP status code.

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);

            return NoContent();
        }
    }
}
