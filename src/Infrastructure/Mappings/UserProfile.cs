using AutoMapper;
using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Infrastructure.Models.Identity;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, User>().ReverseMap();
            CreateMap<ChatUserResponse, User>().ReverseMap()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(source => source.Email)); //Specific Mapping
        }
    }
}