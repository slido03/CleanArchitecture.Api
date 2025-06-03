using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IDocumentTypeRepository
    {
        Task<bool> IsExternalApplicationUsed(int applicationId);
    }
}