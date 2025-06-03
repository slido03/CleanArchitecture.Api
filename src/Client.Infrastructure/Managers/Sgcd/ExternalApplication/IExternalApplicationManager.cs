using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.Import;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetById;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication
{
    public interface IExternalApplicationManager : IManager
    {
        Task<IResult<string>> ExportToExcelAsync(string searchString = "");

        Task<IResult<List<GetAllExternalApplicationsResponse>>> GetAllAsync();

        Task<PaginatedResult<GetAllExternalApplicationsResponse>> GetAllPagedAsync(GetAllExternalApplicationsQuery request);

        Task<IResult<GetExternalApplicationByIdResponse>> GetByIdAsync(GetExternalApplicationByIdQuery request);

        Task<IResult<int>> GetCountAsync();

        Task<IResult<int>> SaveAsync(AddEditExternalApplicationCommand request);

        Task<IResult<int>> ImportAsync(ImportExternalApplicationsCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}
