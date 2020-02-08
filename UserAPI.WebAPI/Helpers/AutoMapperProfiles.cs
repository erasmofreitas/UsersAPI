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
            CreateMap<UserIdentity, UserIdentityDto>().ReverseMap();
            CreateMap<UserIdentity, UserLoginDto>().ReverseMap();
        }
    }
}