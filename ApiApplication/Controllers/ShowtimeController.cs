using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiApplication.Models;
using AutoMapper;
using ApiApplication.Services;
using ApiApplication.Database.Entities;

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

        [HttpGet("date/{date}")]
        public ActionResult<IEnumerable<ShowtimeModel>> GetByDate(DateTime date)
        {
            var entities = _service.GetByDate(date);
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpGet("movie/{title}")]
        public ActionResult<IEnumerable<ShowtimeModel>> GetByTitle(string title)
        {
            var entities = _service.GetByTitle(title);
            var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<ShowtimeModel>>(entities);

            return Ok(models);
        }

        [HttpPost]
        public ActionResult<ShowtimeModel> Create(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);
            var createdEntity = _service.Create(entity);
            var createdModel = _mapper.Map<ShowtimeModel>(entity);

            return CreatedAtAction(nameof(Create), null, createdModel);
        }

        [HttpPut]
        public IActionResult Update(ShowtimeModel model)
        {
            var entity = _mapper.Map<ShowtimeEntity>(model);
            _service.Update(entity);

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
