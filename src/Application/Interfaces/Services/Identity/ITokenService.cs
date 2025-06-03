using CleanArchitecture.Application.Interfaces.Common;
using CleanArchitecture.Application.Requests.Identity;
using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Shared.Wrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}