using AutoMapper;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetById;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Mappings
{
    public class ExternalApplicationProfile : Profile
    {
        public ExternalApplicationProfile()
        {
            CreateMap<AddEditExternalApplicationCommand, ExternalApplication>().ReverseMap();
            CreateMap<ExternalApplication, GetAllExternalApplicationsResponse>().ReverseMap();
            CreateMap<ExternalApplication, GetExternalApplicationByIdResponse>().ReverseMap();
        }
    }
}
