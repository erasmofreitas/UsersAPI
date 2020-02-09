using AutoMapper;
using UserAPI.Domain;
using UserAPI.Domain.Identity;
using UserAPI.WebAPI.Dtos;

namespace UserAPI.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Users, UsersDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
        }
    }
}