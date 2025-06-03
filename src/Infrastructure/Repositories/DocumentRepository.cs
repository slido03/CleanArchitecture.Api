using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IRepositoryAsync<Document, Guid> _repository;

        public DocumentRepository(IRepositoryAsync<Document, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsDocumentTypeUsed(int documentTypeId)
        {
            if (await _repository.GetCountAsync() == 0)
                return false;
            return await _repository.Entities.AnyAsync(d => d.DocumentTypeId == documentTypeId);
        }

        public async Task<bool> IsDocumentExtendedAttributeUsed(Guid documentExtendedAttributeId)
        {
            if (await _repository.GetLongCountAsync() == 0)
                return false;
            return await _repository.Entities.AnyAsync(d => d.ExtendedAttributes.Any(x => x.Id == documentExtendedAttributeId));
        }
    }
}