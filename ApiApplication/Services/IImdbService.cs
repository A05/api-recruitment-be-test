using ApiApplication.Database.Entities;

namespace ApiApplication.Services
{
    public interface IImdbService
    {
        MovieEntity Find(string imdbId);
    }
}
