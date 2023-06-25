using ApiApplication.Database;
using System.Collections.Generic;
using System;
using ApiApplication.Database.Entities;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Domain
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

        public async Task<IEnumerable<ShowtimeEntity>> GetAsync()
        {
            var entities = await _repository.GetCollectionAsync();

            return entities;
        }

        public async Task<ShowtimeEntity> GetByIdAsync(int id)
        {
            var entities = await _repository.GetCollectionAsync(i => i.Id == id);

            return entities.SingleOrDefault();
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetByDateAsync(DateTime date)
        {
            var entities = await _repository.GetCollectionAsync(i => i.StartDate <= date && date <= i.EndDate);

            return entities;
        }

        public async Task<ShowtimeEntity> GetByTitleAsync(string title)
        {
            var entity = await _repository.GetByMovieAsync(movie => movie.Title == title);

            return entity;
        }

        public async Task<(bool, ShowtimeEntity createdEntity)> TryCreateAsync(ShowtimeEntity showtime)
        {
            if (showtime == null)
                throw new ArgumentNullException(nameof(showtime));

            if (string.IsNullOrWhiteSpace(showtime.Movie?.ImdbId))
                throw new ArgumentException($"Movie must be specified.", nameof(showtime));

            EnsureAuditoriumIdIsSupported(showtime.AuditoriumId);

            var existingShowtime = await _repository.GetByMovieAsync(i => i.ImdbId == showtime.Movie.ImdbId);
            if (existingShowtime != null)
                return (false, createdEntity: existingShowtime);

            var movie = await GetMovieFromImdbAsync(showtime.Movie.ImdbId);
            Debug.Assert(movie != null);

            var toBeAddedEntity = showtime.Clone(movie);

            var createdEntity = await _repository.AddAsync(toBeAddedEntity);

            return (true, createdEntity);
        }

        public async Task<(bool, ShowtimeEntity updatedEntity)> TryUpdateAsync(ShowtimeEntity showtime)
        {
            if (showtime == null)
                throw new ArgumentNullException(nameof(showtime));

            if (showtime.Movie != null && string.IsNullOrWhiteSpace(showtime.Movie.ImdbId))
                throw new ArgumentException($"Movie IMDB ID must be specified.", nameof(showtime));

            EnsureAuditoriumIdIsSupported(showtime.AuditoriumId);

            var existingShowtime = await GetByIdAsync(showtime.Id);
            if (existingShowtime == null)
                return (false, updatedEntity: null);

            ShowtimeEntity toBeUpdatedEntity;

            if (showtime.Movie == null)
                toBeUpdatedEntity = showtime.Clone(null);
            else
            {
                var movie = await GetMovieFromImdbAsync(showtime.Movie.ImdbId);
                Debug.Assert(movie != null);

                toBeUpdatedEntity = showtime.Clone(movie);
            }

            var updatedEntity = await _repository.UpdateAsync(toBeUpdatedEntity);

            return (true, updatedEntity);
        }

        public async Task<bool> TryDeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);

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

        private async Task<MovieEntity> GetMovieFromImdbAsync(string imdbId)
        {
            var (movie, description) = await _imdbService.FindAsync(imdbId);

            if (movie == null)
                throw new ApplicationException(description);

            return movie;
        }
    }
}
