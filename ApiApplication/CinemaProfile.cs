using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication
{
    public class CinemaProfile : Profile
    {
        public CinemaProfile()
        {
            CreateMap<ShowtimeEntity, Showtime>();
        }
    }
}
