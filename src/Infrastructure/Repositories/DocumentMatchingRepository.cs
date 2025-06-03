using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    internal class DocumentMatchingRepository : IDocumentMatchingRepository
    {
        private readonly IRepositoryAsync<DocumentMatching, Guid> _repository;

        public DocumentMatchingRepository(IRepositoryAsync<DocumentMatching, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsCentralizedDocumentUsed(Guid centralizedDocumentId)
        {
            if (await _repository.GetLongCountAsync() == 0)
                return false;
            return await _repository.Entities.AnyAsync(m => m.CentralizedDocumentId == centralizedDocumentId);
        }
    }
}
