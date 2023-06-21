using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiApplication.Database;
using ApiApplication.Models;

namespace ApiApplication.Controllers
{
    [Route("api/showtimes")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly CinemaDbContext _context;

        public ShowtimeController(CinemaDbContext context)
        {
            _context = context;
        }

        // GET: api/Showtime
        [HttpGet]
        public ActionResult<IEnumerable<Showtime>> GetShowtime()
        {
            return Ok(_context.Showtimes.ToList());
        }

        // GET: api/Showtime/5
        [HttpGet("{id}")]
        public ActionResult<Showtime> GetShowtime(int id)
        {
            var showtime = _context.Showtimes.Find(id);

            if (showtime == null)
            {
                return NotFound();
            }

            return null;// showtime;
        }

        // PUT: api/Showtime/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShowtime(int id, Showtime showtime)
        {
            if (id != showtime.Id)
            {
                return BadRequest();
            }

            _context.Entry(showtime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowtimeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Showtime
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Showtime>> PostShowtime(Showtime showtime)
        {
            //_context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtime);
        }

        // DELETE: api/Showtime/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            //var showtime = await _context.Showtime.FindAsync(id);
            //if (showtime == null)
            //{
            //    return NotFound();
            //}

            //_context.Showtime.Remove(showtime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShowtimeExists(int id)
        {
            return true;// _context.Showtime.Any(e => e.Id == id);
        }
    }
}
