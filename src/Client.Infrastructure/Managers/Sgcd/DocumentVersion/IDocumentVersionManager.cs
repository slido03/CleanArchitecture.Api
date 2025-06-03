using CleanArchitecture.Application.Features.DocumentVersions.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAllByDocument;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCountByDocument;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentVersion
{
    public interface IDocumentVersionManager : IManager
    {
        Task<IResult<List<GetAllDocumentVersionsResponse>>> GetAllAsync();

        Task<IResult<List<GetAllDocumentVersionsByDocumentResponse>>> GetAllByDocumentAsync(GetAllDocumentVersionsByDocumentQuery request);

        Task<PaginatedResult<GetAllDocumentVersionsResponse>> GetAllPagedAsync(GetAllDocumentVersionsQuery request);

        Task<PaginatedResult<GetAllDocumentVersionsByDocumentResponse>> GetAllPagedByDocumentAsync(GetAllDocumentVersionsByDocumentQuery request);

        Task<IResult<GetDocumentVersionByIdResponse>> GetByIdAsync(GetDocumentVersionByIdQuery request);

        Task<IResult<long>> GetCount();

        Task<IResult<int>> GetCountByDocumentAsync(GetDocumentVersionCountByDocumentQuery request);

        Task<IResult<Guid>> SaveAsync(AddEditDocumentVersionCommand request);

        Task<IResult<Guid>> DeleteAsync(Guid id);
    }
}
