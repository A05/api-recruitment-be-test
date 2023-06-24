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

        bool TryCreate(ShowtimeEntity showtime, out ShowtimeEntity createdEntity);

        bool TryUpdate(ShowtimeEntity showtime, out ShowtimeEntity updatedEntity);

        void Delete(int id);
    }
}
