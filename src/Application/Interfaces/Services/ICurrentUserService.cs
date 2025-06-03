using CleanArchitecture.Application.Interfaces.Common;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}