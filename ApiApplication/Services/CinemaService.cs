using ApiApplication.Database;
using System.Collections.Generic;
using System;
using ApiApplication.Database.Entities;
using System.Diagnostics;
using System.Linq;

namespace ApiApplication.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IImdbService _imdbService;
        private readonly IShowtimesRepository _repository;

        public CinemaService(IImdbService imdbService, IShowtimesRepository repository)
        {
            _imdbService = imdbService ?? throw new ArgumentNullException(nameof(imdbService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<ShowtimeEntity> Get()
        {
            var entities = _repository.GetCollection();

            return entities;
        }

        public ShowtimeEntity GetById(int id)
        {
            var entity = _repository.GetCollection(i => i.Id == id).SingleOrDefault();

            return entity;
        }

        public IEnumerable<ShowtimeEntity> GetByDate(DateTime date)
        {
            var entities = _repository.GetCollection(i => i.StartDate <= date && date <= i.EndDate);

            return entities;
        }

        public ShowtimeEntity GetByTitle(string title)
        {
            var entity = _repository.GetByMovie(movie => movie.Title == title);

            return entity;
        }

        public bool TryCreate(ShowtimeEntity showtime, out ShowtimeEntity createdEntity)
        {
            if (showtime == null)
                throw new ArgumentNullException(nameof(showtime));

            if (string.IsNullOrWhiteSpace(showtime.Movie?.ImdbId))
                throw new ArgumentException($"Movie must be specified.", nameof(showtime));

            EnsureAuditoriumIdIsSupported(showtime.AuditoriumId);

            var existingShowtime = _repository.GetByMovie(i => i.ImdbId == showtime.Movie.ImdbId);
            if (existingShowtime != null)
            {
                createdEntity = existingShowtime;
                return false;
            }

            var movie = GetMovieFromImdb(showtime.Movie.ImdbId);
            Debug.Assert(movie != null);

            var toBeAddedEntity = showtime.Clone(movie);

            createdEntity = _repository.Add(toBeAddedEntity);

            return true;
        }

        public bool TryUpdate(ShowtimeEntity showtime, out ShowtimeEntity updatedEntity)
        {
            if (showtime == null)
                throw new ArgumentNullException(nameof(showtime));

            if (showtime.Movie != null && string.IsNullOrWhiteSpace(showtime.Movie.ImdbId))
                throw new ArgumentException($"Movie IMDB ID must be specified.", nameof(showtime));

            EnsureAuditoriumIdIsSupported(showtime.AuditoriumId);

            var existingShowtime = _repository.GetCollection(i => i.Id == showtime.Id).SingleOrDefault();
            if (existingShowtime == null)
            {
                updatedEntity = null;
                return false;
            }

            ShowtimeEntity toBeUpdatedEntity;

            if (showtime.Movie == null)
                toBeUpdatedEntity = showtime.Clone(null);
            else
            {
                var movie = GetMovieFromImdb(showtime.Movie.ImdbId);
                Debug.Assert(movie != null);

                toBeUpdatedEntity = showtime.Clone(movie);
            }

            updatedEntity = _repository.Update(toBeUpdatedEntity);

            return true;
        }

        public bool TryDelete(int id)
        {
            try
            {
                _repository.Delete(id);

                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private void EnsureAuditoriumIdIsSupported(int auditoriumId)
        {
            if (auditoriumId != 1 && auditoriumId != 2 && auditoriumId != 3)
                throw new ArgumentException($"The {auditoriumId} is not supported yet.");
        }

        private MovieEntity GetMovieFromImdb(string imdbId)
        {
            var movie = _imdbService.Find(imdbId, out var description);
            if (movie == null)
                throw new ApplicationException(description);

            return movie;
        }
    }
}
