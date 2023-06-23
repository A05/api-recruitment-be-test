using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ShowtimeEntity, Showtime>();
        }
    }
}
