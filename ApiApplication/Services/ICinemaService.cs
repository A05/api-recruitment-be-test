using ApiApplication.Database.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public interface ICinemaService
    {
        Task<IEnumerable<ShowtimeEntity>> GetAsync();

        Task<ShowtimeEntity> GetByIdAsync(int id);

        Task<IEnumerable<ShowtimeEntity>> GetByDateAsync(DateTime date);

        Task<ShowtimeEntity> GetByTitleAsync(string title);

        Task<(bool, ShowtimeEntity createdEntity)> TryCreateAsync(ShowtimeEntity showtime);

        Task<(bool, ShowtimeEntity updatedEntity)> TryUpdateAsync(ShowtimeEntity showtime);

        Task<bool> TryDeleteAsync(int id);
    }
}
