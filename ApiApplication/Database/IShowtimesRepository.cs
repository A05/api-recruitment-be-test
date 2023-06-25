﻿using ApiApplication.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApiApplication.Database
{
    public interface IShowtimesRepository
    {
        Task<IEnumerable<ShowtimeEntity>> GetCollectionAsync();
        Task<IEnumerable<ShowtimeEntity>> GetCollectionAsync(Expression<Func<ShowtimeEntity, bool>> filter);
        Task<ShowtimeEntity> GetByMovieAsync(Expression<Func<MovieEntity, bool>> filter);
        Task<ShowtimeEntity> AddAsync(ShowtimeEntity entity);
        Task<ShowtimeEntity> UpdateAsync(ShowtimeEntity entity);
        Task<ShowtimeEntity> DeleteAsync(int id);
    }
}
