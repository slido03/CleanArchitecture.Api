using CleanArchitecture.Application.Features.Dashboards.Queries.GetData;
using CleanArchitecture.Shared.Wrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<GetDashboardDataResponse>> GetDataAsync();
    }
}