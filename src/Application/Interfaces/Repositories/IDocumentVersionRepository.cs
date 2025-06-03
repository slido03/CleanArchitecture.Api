using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IDocumentVersionRepository
    {
        Task<bool> IsDocumentUsed(Guid documentId);
    }
}
