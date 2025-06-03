using CleanArchitecture.Application.Features.DocumentVersions.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAllByDocument;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCountByDocument;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Client.Infrastructure.Routes;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentVersion
{
    public class DocumentVersionManager : IDocumentVersionManager
    {
        private readonly HttpClient _httpClient;

        public DocumentVersionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllDocumentVersionsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetAll);
            return await response.ToResult<List<GetAllDocumentVersionsResponse>>();
        }

        public async Task<IResult<List<GetAllDocumentVersionsByDocumentResponse>>> GetAllByDocumentAsync(GetAllDocumentVersionsByDocumentQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetAllByDocument(request.DocumentId));
            return await response.ToResult<List<GetAllDocumentVersionsByDocumentResponse>>();
        }

        public async Task<PaginatedResult<GetAllDocumentVersionsResponse>> GetAllPagedAsync(GetAllDocumentVersionsQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentVersionsResponse>();
        }

        public async Task<PaginatedResult<GetAllDocumentVersionsByDocumentResponse>> GetAllPagedByDocumentAsync(GetAllDocumentVersionsByDocumentQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetAllPagedByDocument(request.DocumentId, request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentVersionsByDocumentResponse>();
        }

        public async Task<IResult<GetDocumentVersionByIdResponse>> GetByIdAsync(GetDocumentVersionByIdQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentVersionByIdResponse>();
        }

        public async Task<IResult<long>> GetCount()
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetCount);
            return await response.ToResult<long>();
        }

        public async Task<IResult<int>> GetCountByDocumentAsync(GetDocumentVersionCountByDocumentQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentVersionsEndpoints.GetCountByDocument(request.DocumentId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<Guid>> SaveAsync(AddEditDocumentVersionCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(DocumentVersionsEndpoints.Save, request);
            return await response.ToResult<Guid>();
        }

        public async Task<IResult<Guid>> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{DocumentVersionsEndpoints.Delete}/{id}");
            return await response.ToResult<Guid>();
        }
    }
}
