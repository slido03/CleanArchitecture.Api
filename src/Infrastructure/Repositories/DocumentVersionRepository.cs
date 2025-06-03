using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class DocumentVersionRepository : IDocumentVersionRepository
    {
        private readonly IRepositoryAsync<DocumentVersion, Guid> _repository;

        public DocumentVersionRepository(IRepositoryAsync<DocumentVersion, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsDocumentUsed(Guid documentId)
        {
            if (await _repository.GetLongCountAsync() == 0)
                return false;
            return await _repository.Entities.AnyAsync(v => v.DocumentId == documentId);
        }
    }
}
