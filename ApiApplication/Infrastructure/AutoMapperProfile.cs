using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;
using System;

namespace ApiApplication
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ShowtimeEntity, ShowtimeModel>()
                .ForMember(d => d.Schedule, opt => opt.MapFrom(s => string.Join(",", s.Schedule)));

            CreateMap<ShowtimeModel, ShowtimeEntity>()
                .ForMember(d => d.Schedule, opt => opt.MapFrom(s => s.Schedule.Split(",", StringSplitOptions.RemoveEmptyEntries)));

            CreateMap<MovieEntity, MovieModel>()
                .ForSourceMember(s => s.Id, opt => opt.DoNotValidate())
                .ForMember(d => d.Starts, opt => opt.MapFrom(s => s.Stars));

            CreateMap<MovieModel, MovieEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Stars, opt => opt.MapFrom(s => s.Starts));
        }
    }
}
