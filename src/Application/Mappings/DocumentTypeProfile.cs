using AutoMapper;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAllByExternalApplication;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitecture.Application.Models.Sgcd;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<ImportDocumentTypes, DocumentType>().ReverseMap();
            CreateMap<DocumentType, GetDocumentTypeByIdResponse>()
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.ExternalApplication.Name));
            CreateMap<DocumentType, GetAllDocumentTypesResponse>()
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.ExternalApplication.Name));
            CreateMap<DocumentType, GetAllDocumentTypesByExternalApplicationResponse>()
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.ExternalApplication.Name));
        }
    }
}