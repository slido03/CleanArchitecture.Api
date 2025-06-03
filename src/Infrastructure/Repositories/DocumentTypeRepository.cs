using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly IRepositoryAsync<DocumentType, int> _repository;

        public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsExternalApplicationUsed(int applicationId)
        {
            if (await _repository.GetCountAsync() == 0)
                return false;
            return await _repository.Entities.AnyAsync(t => t.ExternalApplicationId == applicationId);
        }
    }
}