using AutoMapper;
using CleanArchitecture.Application.Responses.Audit;
using CleanArchitecture.Infrastructure.Models.Audit;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}