using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task<bool> IsDocumentTypeUsed(int documentTypeId);

        Task<bool> IsDocumentExtendedAttributeUsed(Guid documentExtendedAttributeId);
    }
}