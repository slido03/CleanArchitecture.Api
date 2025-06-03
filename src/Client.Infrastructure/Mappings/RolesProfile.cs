using AutoMapper;
using CleanArchitecture.Application.Requests.Identity;
using CleanArchitecture.Application.Responses.Identity;

namespace CleanArchitecture.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}