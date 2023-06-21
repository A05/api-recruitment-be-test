using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ApiApplication.Database
{
    public class ShowtimesRepository : IShowtimesRepository
    {
        private readonly CinemaDbContext _context;

        public ShowtimesRepository(CinemaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ShowtimeEntity Add(ShowtimeEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Movie == null)
                throw new ArgumentException("Movie must be specified.", nameof(entity));

            var clone = entity.Clone();

            _context.Showtimes.Add(clone);
            _context.Movies.Add(clone.Movie);
            _context.SaveChanges();

            return clone;
        }

        public ShowtimeEntity Delete(int id)
        {
            var entity = _context.Showtimes.Include(i => i.Movie).Where(i => i.Id == id).Single();

            _context.Showtimes.Remove(entity);
            _context.Movies.Remove(entity.Movie);
            _context.SaveChanges();

            return entity;
        }

        public ShowtimeEntity GetByMovie(Expression<Func<MovieEntity, bool>> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            // TODO: Do it with one round trip.

            var movie = _context.Movies.Where(filter).FirstOrDefault();
            if (movie == null)
                return null;

            return _context.Showtimes.FirstOrDefault(i => i.Id == movie.ShowtimeId);
        }

        public IEnumerable<ShowtimeEntity> GetCollection()
        {
            return GetCollection(null);
        }

        public IEnumerable<ShowtimeEntity> GetCollection(Expression<Func<ShowtimeEntity, bool>> filter)
        {
            IQueryable<ShowtimeEntity> query = _context.Showtimes.Include(i => i.Movie);

            if (filter != null)
                query = query.Where(filter);

            return query.ToArray();
        }

        public ShowtimeEntity Update(ShowtimeEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Debug.Assert(_context != null);

            var query = _context.Showtimes.Where(i => i.Id == entity.Id);
            
            if (entity.Movie != null)            
                query = query.Include(i => i.Movie);
            
            var entityToUpdate = query.FirstOrDefault();
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

            _context.SaveChanges();

            return entityToUpdate;
        }
    }
}
