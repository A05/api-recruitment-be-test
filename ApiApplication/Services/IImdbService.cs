using ApiApplication.Database.Entities;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public interface IImdbService
    {
        Task<(MovieEntity, string description)> FindAsync(string imdbId);
    }
}
