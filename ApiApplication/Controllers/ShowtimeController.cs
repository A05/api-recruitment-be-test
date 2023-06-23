using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ApiApplication.Database;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimesRepository _repository;
        private readonly IMapper _mapper;

        public ShowtimeController(IShowtimesRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Showtime>> Get()
        {
            //var entities = _repository.GetCollection();
            //var models = _mapper.Map<IEnumerable<ShowtimeEntity>, IEnumerable<Showtime>>(entities);

            //return Ok(models);
            return Ok(Enumerable.Empty<Showtime>());
        }

        [HttpGet("date/{date}")]
        public ActionResult<IEnumerable<Showtime>> GetByDate(DateTime date)
        {
            return Ok(Enumerable.Empty<Showtime>());
        }

        [HttpGet("movie/{title}")]
        public ActionResult<IEnumerable<Showtime>> GetByTitle(string title)
        {
            return Ok(Enumerable.Empty<Showtime>());
        }

        [HttpPost]
        public ActionResult<Showtime> Create(Showtime showtime)
        {
            //return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtime);
            throw new NotImplementedException();
        }

        [HttpPut]
        public IActionResult Update(Showtime showtime)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}
