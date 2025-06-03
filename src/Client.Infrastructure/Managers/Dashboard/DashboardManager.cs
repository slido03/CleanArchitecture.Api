using CleanArchitecture.Application.Features.Dashboards.Queries.GetData;
using CleanArchitecture.Client.Infrastructure.Extensions;
using CleanArchitecture.Shared.Wrapper;
using System.Net.Http;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        private readonly HttpClient _httpClient;

        public DashboardManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<GetDashboardDataResponse>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DashboardEndpoints.GetData);
            var data = await response.ToResult<GetDashboardDataResponse>();
            return data;
        }
    }
}