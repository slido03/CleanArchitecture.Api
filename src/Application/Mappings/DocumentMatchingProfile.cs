using AutoMapper;
using CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetByCentralizedDocument;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Mappings
{
    public class DocumentMatchingProfile : Profile
    {
        public DocumentMatchingProfile()
        {
            CreateMap<AddEditDocumentMatchingCommand, DocumentMatching>().ReverseMap();
            CreateMap<DocumentMatching, GetAllDocumentMatchingsResponse>()
                .ForMember(dest => dest.CentralizedDocument, opt => opt.MapFrom(src => src.CentralizedDocument.Title));
            CreateMap<DocumentMatching, GetDocumentMatchingByCentralizedDocumentResponse>()
                .ForMember(dest => dest.CentralizedDocument, opt => opt.MapFrom(src => src.CentralizedDocument.Title));
            CreateMap<DocumentMatching, GetDocumentMatchingByIdResponse>()
                .ForMember(dest => dest.CentralizedDocument, opt => opt.MapFrom(src => src.CentralizedDocument.Title));
        }
    }
}
