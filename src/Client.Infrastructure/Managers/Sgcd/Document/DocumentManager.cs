using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.Documents.Commands.RestoreVersion;
using CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using CleanArchitecture.Application.Features.Documents.Queries.GetAllByDocumentType;
using CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId;
using CleanArchitecture.Application.Features.Documents.Queries.GetById;
using CleanArchitecture.Application.Features.Documents.Queries.GetCount;
using CleanArchitecture.Application.Features.Documents.Queries.GetCountByDocumentType;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Client.Infrastructure.Routes;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document
{
    public class DocumentManager : IDocumentManager
    {
        private readonly HttpClient _httpClient;

        public DocumentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllDocumentsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetAll);
            return await response.ToResult<List<GetAllDocumentsResponse>>();
        }

        public async Task<PaginatedResult<GetAllDocumentsResponse>> GetAllPagedAsync(GetAllDocumentsQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentsResponse>();
        }

        public async Task<IResult<List<GetAllDocumentsByDocumentTypeResponse>>> GetAllByDocumentTypeAsync(GetAllDocumentsByDocumentTypeQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetAllByDocumentType(request.DocumentTypeId));
            return await response.ToResult<List<GetAllDocumentsByDocumentTypeResponse>>();
        }

        public async Task<PaginatedResult<GetAllDocumentsByDocumentTypeResponse>> GetAllPagedByDocumentTypeAsync(GetAllDocumentsByDocumentTypeQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetAllPagedByDocumentType(request.DocumentTypeId, request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentsByDocumentTypeResponse>();
        }

        public async Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentByIdResponse>();
        }

        public async Task<IResult<GetDocumentByExternalIdResponse>> GetByExternalIdAsync(GetDocumentByExternalIdQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetByExternalId(request.DocumentType, request.ExternalId));
            return await response.ToResult<GetDocumentByExternalIdResponse>();
        }

        public async Task<IResult<long>> GetCountAsync(GetDocumentCountQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetCount);
            return await response.ToResult<long>();
        }

        public async Task<IResult<int>> GetCountByDocumentTypeAsync(GetDocumentCountByDocumentTypeQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentsEndpoints.GetCountByDocumentType(request.DocumentTypeId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<Guid>> RestoreVersionAsync(RestoreDocumentVersionCommand request)
        {
            var response = await _httpClient.PutAsJsonAsync(DocumentsEndpoints.RestoreVersion, request);
            return await response.ToResult<Guid>();
        }

        public async Task<IResult<Guid>> SaveAsync(AddEditDocumentCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(DocumentsEndpoints.Save, request);
            return await response.ToResult<Guid>();
        }

        public async Task<IResult<Guid>> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{DocumentsEndpoints.Delete}/{id}");
            return await response.ToResult<Guid>();
        }
    }
}