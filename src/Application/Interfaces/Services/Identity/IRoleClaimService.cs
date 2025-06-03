using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Common;
using CleanArchitecture.Application.Requests.Identity;
using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Shared.Wrapper;

namespace CleanArchitecture.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}