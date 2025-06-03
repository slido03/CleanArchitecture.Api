using AutoMapper;
using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Infrastructure.Models.Identity;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, Role>().ReverseMap();
        }
    }
}