using AutoMapper;
using Yggdrasil.Data.Entity;
using YggdrasilApi.Dto;

namespace YggdrasilApi.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MovieDto, Movie>().ForMember(x => x.BoxOffice, o => o.MapFrom(src => src.BoxOffice)); ;
            CreateMap<Movie, MovieDto>().ForMember(x => x.BoxOffice, o => o.MapFrom(src => src.BoxOffice));
            CreateMap<ActorDto, Actor>().ReverseMap();
            CreateMap<DirectorDto, Director>().ReverseMap();
        }
    }
}
