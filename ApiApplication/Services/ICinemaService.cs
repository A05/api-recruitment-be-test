using ApiApplication.Database.Entities;
using System.Collections.Generic;
using System;

namespace ApiApplication.Services
{
    public interface ICinemaService
    {
        IEnumerable<ShowtimeEntity> Get();

        ShowtimeEntity GetById(int id);

        IEnumerable<ShowtimeEntity> GetByDate(DateTime date);

        ShowtimeEntity GetByTitle(string title);

        ShowtimeEntity Create(ShowtimeEntity showtime);

        ShowtimeEntity Update(ShowtimeEntity showtime);

        void Delete(int id);
    }
}
