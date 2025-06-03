using AutoMapper;
using CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using CleanArchitecture.Application.Features.Documents.Queries.GetAllByDocumentType;
using CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId;
using CleanArchitecture.Application.Features.Documents.Queries.GetById;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, GetDocumentByIdResponse>()
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.Name))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.DocumentMatching.ExternalId))
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.DocumentType.ExternalApplication.Name));
            CreateMap<Document, GetAllDocumentsResponse>()
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.Name))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.DocumentMatching.ExternalId))
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.DocumentType.ExternalApplication.Name));
            CreateMap<Document, GetAllDocumentsByDocumentTypeResponse>()
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.Name))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.DocumentMatching.ExternalId))
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.DocumentType.ExternalApplication.Name));
            CreateMap<Document, GetDocumentByExternalIdResponse>()
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.Name))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.DocumentMatching.ExternalId))
                .ForMember(dest => dest.ExternalApplication, opt => opt.MapFrom(src => src.DocumentType.ExternalApplication.Name))
                .ForMember(dest => dest.ExternalApplicationId, opt => opt.MapFrom(src => src.DocumentType.ExternalApplication.Id));
        }
    }
}