using CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetByCentralizedDocument;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Client.Infrastructure.Routes;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentMatching
{
    public class DocumentMatchingManager : IDocumentMatchingManager
    {
        private readonly HttpClient _httpClient;

        public DocumentMatchingManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllDocumentMatchingsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(DocumentMatchingsEndpoints.GetAll);
            return await response.ToResult<List<GetAllDocumentMatchingsResponse>>();
        }

        public async Task<PaginatedResult<GetAllDocumentMatchingsResponse>> GetAllPagedAsync(GetAllDocumentMatchingsQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentMatchingsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentMatchingsResponse>();
        }

        public async Task<IResult<GetDocumentMatchingByCentralizedDocumentResponse>> GetByCentralizedDocumentAsync(GetDocumentMatchingByCentralizedDocumentQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentMatchingsEndpoints.GetByCentralizedDocument(request.CentralizedDocumentId));
            return await response.ToResult<GetDocumentMatchingByCentralizedDocumentResponse>();
        }

        public async Task<IResult<GetDocumentMatchingByIdResponse>> GetByIdAsync(GetDocumentMatchingByIdQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentMatchingsEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentMatchingByIdResponse>();
        }

        public async Task<IResult<long>> GetCount()
        {
            var response = await _httpClient.GetAsync(DocumentMatchingsEndpoints.GetCount);
            return await response.ToResult<long>();
        }

        public async Task<IResult<Guid>> SaveAsync(AddEditDocumentMatchingCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(DocumentMatchingsEndpoints.Save, request);
            return await response.ToResult<Guid>();
        }

        public async Task<IResult<Guid>> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{DocumentMatchingsEndpoints.Delete}/{id}");
            return await response.ToResult<Guid>();
        }
    }
}
