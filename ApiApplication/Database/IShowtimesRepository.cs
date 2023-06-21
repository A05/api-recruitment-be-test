using ApiApplication.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ApiApplication.Database
{
    public interface IShowtimesRepository
    {
        IEnumerable<ShowtimeEntity> GetCollection();
        IEnumerable<ShowtimeEntity> GetCollection(Expression<Func<ShowtimeEntity, bool>> filter);
        ShowtimeEntity GetByMovie(Expression<Func<MovieEntity, bool>> filter);
        ShowtimeEntity Add(ShowtimeEntity entity);
        ShowtimeEntity Update(ShowtimeEntity entity);
        ShowtimeEntity Delete(int id);
    }
}
