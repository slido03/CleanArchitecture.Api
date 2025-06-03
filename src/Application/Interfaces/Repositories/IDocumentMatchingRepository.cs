using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IDocumentMatchingRepository
    {
        Task<bool> IsCentralizedDocumentUsed(Guid centralizedDocumentId);
    }
}
