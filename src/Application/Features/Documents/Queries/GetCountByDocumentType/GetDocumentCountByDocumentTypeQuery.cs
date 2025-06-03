using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Queries.GetCountByDocumentType
{
    public class GetDocumentCountByDocumentTypeQuery : IRequest<Result<int>>
    {
        public int DocumentTypeId { get; set; }

        public GetDocumentCountByDocumentTypeQuery(int documentTypeId)
        {
            DocumentTypeId = documentTypeId;
        }
    }

    internal class GetDocumentCountByDocumentTypeQueryHandler : IRequestHandler<GetDocumentCountByDocumentTypeQuery, Result<int>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetDocumentCountByDocumentTypeQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetDocumentCountByDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            var documentCount = await _unitOfWork.Repository<Document>().Entities
                .Where(d => d.DocumentTypeId == request.DocumentTypeId)
                .CountAsync(cancellationToken);
            return await Result<int>.SuccessAsync(documentCount);
        }
    }
}
