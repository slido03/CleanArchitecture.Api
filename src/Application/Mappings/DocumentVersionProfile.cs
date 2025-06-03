using AutoMapper;
using CleanArchitecture.Application.Features.DocumentVersions.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Mappings
{
    public class DocumentVersionProfile : Profile
    {
        public DocumentVersionProfile()
        {
            CreateMap<AddEditDocumentVersionCommand, DocumentVersion>().ReverseMap();
            CreateMap<DocumentVersion, GetDocumentVersionByIdResponse>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Title));
            CreateMap<DocumentVersion, GetAllDocumentVersionsResponse>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Title));
        }
    }
}
