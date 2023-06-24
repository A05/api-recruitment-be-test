using ApiApplication.Database.Entities;
using System.Collections.Generic;
using System;

namespace ApiApplication.Services
{
    public interface ICinemaService
    {
        IEnumerable<ShowtimeEntity> Get();
        
        IEnumerable<ShowtimeEntity> GetByDate(DateTime date);

        IEnumerable<ShowtimeEntity> GetByTitle(string title);

        ShowtimeEntity Create(ShowtimeEntity showtime);

        void Update(ShowtimeEntity showtime);

        void Delete(int id);
    }
}
