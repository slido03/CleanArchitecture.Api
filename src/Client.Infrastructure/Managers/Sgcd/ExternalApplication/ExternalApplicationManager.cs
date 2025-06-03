using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.Import;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetById;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Client.Infrastructure.Routes;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication
{
    public class ExternalApplicationManager : IExternalApplicationManager
    {
        private readonly HttpClient _httpClient;

        public ExternalApplicationManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? ExternalApplicationsEndpoints.Export
                : ExternalApplicationsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<List<GetAllExternalApplicationsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ExternalApplicationsEndpoints.GetAll);
            return await response.ToResult<List<GetAllExternalApplicationsResponse>>();
        }

        public async Task<PaginatedResult<GetAllExternalApplicationsResponse>> GetAllPagedAsync(GetAllExternalApplicationsQuery request)
        {
            var response = await _httpClient.GetAsync(ExternalApplicationsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.OrderBy));
            return await response.ToPaginatedResult<GetAllExternalApplicationsResponse>();
        }

        public async Task<IResult<GetExternalApplicationByIdResponse>> GetByIdAsync(GetExternalApplicationByIdQuery request)
        {
            var response = await _httpClient.GetAsync(ExternalApplicationsEndpoints.GetById(request.Id));
            return await response.ToResult<GetExternalApplicationByIdResponse>();
        }

        public async Task<IResult<int>> GetCountAsync()
        {
            var response = await _httpClient.GetAsync(ExternalApplicationsEndpoints.GetCount);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditExternalApplicationCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(ExternalApplicationsEndpoints.Save, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> ImportAsync(ImportExternalApplicationsCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ExternalApplicationsEndpoints.Import, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ExternalApplicationsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }
    }
}
