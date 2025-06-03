using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentTypes.Commands.Import;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAllByExternalApplication;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCountByExternalApplication;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Client.Infrastructure.Routes;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType
{
    public class DocumentTypeManager : IDocumentTypeManager
    {
        private readonly HttpClient _httpClient;

        public DocumentTypeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? DocumentTypesEndpoints.Export
                : DocumentTypesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAll);
            return await response.ToResult<List<GetAllDocumentTypesResponse>>();
        }

        public async Task<IResult<List<GetAllDocumentTypesByExternalApplicationResponse>>> GetAllByExternalApplicationAsync(GetAllDocumentTypesByExternalApplicationQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAllByExternalApplication(request.ExternalApplicationId));
            return await response.ToResult<List<GetAllDocumentTypesByExternalApplicationResponse>>();
        }


        public async Task<PaginatedResult<GetAllDocumentTypesResponse>> GetAllPagedAsync(GetAllDocumentTypesQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentTypesResponse>();
        }

        public async Task<PaginatedResult<GetAllDocumentTypesByExternalApplicationResponse>> GetAllPagedByExternalApplicationAsync(GetAllDocumentTypesByExternalApplicationQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetAllPagedByExternalApplication(request.ExternalApplicationId, request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllDocumentTypesByExternalApplicationResponse>();
        }

        public async Task<IResult<GetDocumentTypeByIdResponse>> GetByIdAsync(GetDocumentTypeByIdQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentTypeByIdResponse>();
        }

        public async Task<IResult<int>> GetCount()
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetCount);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> GetCountByExternalApplicationAsync(GetDocumentTypeCountByExternalApplicationQuery request)
        {
            var response = await _httpClient.GetAsync(DocumentTypesEndpoints.GetCountByExternalApplication(request.ExternalApplicationId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(DocumentTypesEndpoints.Save, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> ImportAsync(ImportDocumentTypesCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentTypesEndpoints.Import, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{DocumentTypesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }
    }
}