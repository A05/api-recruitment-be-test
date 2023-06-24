using ApiApplication.Database;
using System.Collections.Generic;
using System.Linq;
using System;
using ApiApplication.Database.Entities;

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
            throw new NotImplementedException();
        }

        public IEnumerable<ShowtimeEntity> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ShowtimeEntity> GetByTitle(string title)
        {
            throw new NotImplementedException();
        }

        public ShowtimeEntity Create(ShowtimeEntity showtime)
        {
            throw new NotImplementedException();
        }

        public void Update(ShowtimeEntity showtime)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
