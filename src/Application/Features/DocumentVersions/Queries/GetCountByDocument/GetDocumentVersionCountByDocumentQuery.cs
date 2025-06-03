using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCountByDocument
{
    public class GetDocumentVersionCountByDocumentQuery : IRequest<Result<int>>
    {
        public Guid DocumentId { get; set; }

        public GetDocumentVersionCountByDocumentQuery(Guid documentId)
        {
            DocumentId = documentId;
        }
    }

    internal class GetDocumentVersionCountByDocumentIdQueryHandler : IRequestHandler<GetDocumentVersionCountByDocumentQuery, Result<int>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetDocumentVersionCountByDocumentIdQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetDocumentVersionCountByDocumentQuery request, CancellationToken cancellationToken)
        {
            var documentVersionCount = await _unitOfWork.Repository<DocumentVersion>().Entities
                .Where(v => v.DocumentId == request.DocumentId)
                .CountAsync(cancellationToken);
            return await Result<int>.SuccessAsync(documentVersionCount);
        }
    }
}
