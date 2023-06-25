using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApiApplication.Database
{
    public class ShowtimesRepository : IShowtimesRepository
    {
        private readonly CinemaDbContext _context;

        public ShowtimesRepository(CinemaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ShowtimeEntity> AddAsync(ShowtimeEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Movie == null)
                throw new ArgumentException("Movie must be specified.", nameof(entity));

            var clone = entity.Clone();

            _context.Showtimes.Add(clone);
            _context.Movies.Add(clone.Movie);
            await _context.SaveChangesAsync();

            return clone;
        }

        public async Task<ShowtimeEntity> DeleteAsync(int id)
        {
            var entity = _context.Showtimes.Include(i => i.Movie).Where(i => i.Id == id).Single();

            _context.Showtimes.Remove(entity);
            _context.Movies.Remove(entity.Movie);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<ShowtimeEntity> GetByMovieAsync(Expression<Func<MovieEntity, bool>> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            // TODO: Do it with one round trip.

            var movie = await _context.Movies.Where(filter).FirstOrDefaultAsync();
            if (movie == null)
                return null;

            return await _context.Showtimes.FirstOrDefaultAsync(i => i.Id == movie.ShowtimeId);
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetCollectionAsync()
        {
            return await GetCollectionAsync(null);
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetCollectionAsync(Expression<Func<ShowtimeEntity, bool>> filter)
        {
            IQueryable<ShowtimeEntity> query = _context.Showtimes.Include(i => i.Movie);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToArrayAsync();
        }

        public async Task<ShowtimeEntity> UpdateAsync(ShowtimeEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Debug.Assert(_context != null);

            var query = _context.Showtimes.Where(i => i.Id == entity.Id);
            
            if (entity.Movie != null)            
                query = query.Include(i => i.Movie);
            
            var entityToUpdate = await query.FirstOrDefaultAsync();
            if (entityToUpdate == null)
                throw new ArgumentException($"Failed to find a showtime with ID = {entity.Id}.", nameof(entity));

            entityToUpdate.StartDate = entity.StartDate;
            entityToUpdate.EndDate = entity.EndDate;
            entityToUpdate.Schedule = entity.Schedule;
            entityToUpdate.AuditoriumId = entity.AuditoriumId;

            if (entity.Movie != null)
            {
                Debug.Assert(entityToUpdate.Movie != null);

                entityToUpdate.Movie.ImdbId = entity.Movie.ImdbId;
                entityToUpdate.Movie.Title = entity.Movie.Title;
                entityToUpdate.Movie.Stars = entity.Movie.Stars;
                entityToUpdate.Movie.ReleaseDate = entity.Movie.ReleaseDate;
            }

            await _context.SaveChangesAsync();

            return entityToUpdate;
        }
    }
}
