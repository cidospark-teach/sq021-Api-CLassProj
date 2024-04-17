using AutoMapper;
using WebApplication2.Models.DTOs;
using WebApplication2.Models.Entities;

namespace WebApplication2.ConfigsSettings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, UserToReturnDto>().ReverseMap();
            CreateMap<UserToAddDto, AppUser>().ReverseMap();
            CreateMap<AppUser, UserToReturnDto>().ReverseMap();
        }
    }
}
