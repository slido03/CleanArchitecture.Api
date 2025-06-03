using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.Documents.Commands.RestoreVersion;
using CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using CleanArchitecture.Application.Features.Documents.Queries.GetAllByDocumentType;
using CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId;
using CleanArchitecture.Application.Features.Documents.Queries.GetById;
using CleanArchitecture.Application.Features.Documents.Queries.GetCount;
using CleanArchitecture.Application.Features.Documents.Queries.GetCountByDocumentType;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document
{
    public interface IDocumentManager : IManager
    {
        Task<IResult<List<GetAllDocumentsResponse>>> GetAllAsync();

        Task<IResult<List<GetAllDocumentsByDocumentTypeResponse>>> GetAllByDocumentTypeAsync(GetAllDocumentsByDocumentTypeQuery request);

        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllPagedAsync(GetAllDocumentsQuery request);

        Task<PaginatedResult<GetAllDocumentsByDocumentTypeResponse>> GetAllPagedByDocumentTypeAsync(GetAllDocumentsByDocumentTypeQuery request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<GetDocumentByExternalIdResponse>> GetByExternalIdAsync(GetDocumentByExternalIdQuery request);

        Task<IResult<long>> GetCountAsync(GetDocumentCountQuery request);

        Task<IResult<int>> GetCountByDocumentTypeAsync(GetDocumentCountByDocumentTypeQuery request);

        Task<IResult<Guid>> RestoreVersionAsync(RestoreDocumentVersionCommand request);

        Task<IResult<Guid>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<Guid>> DeleteAsync(Guid id);
    }
}