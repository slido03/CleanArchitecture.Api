using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCount
{
    public class GetDocumentVersionCountQuery : IRequest<Result<long>>
    {
        public GetDocumentVersionCountQuery() 
        {
        }
    }

    internal class GetDocumentVersionCountQueryHandler : IRequestHandler<GetDocumentVersionCountQuery, Result<long>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetDocumentVersionCountQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(GetDocumentVersionCountQuery request, CancellationToken cancellationToken)
        {
            var documentVersionCount = await _unitOfWork.Repository<DocumentVersion>().GetLongCountAsync();
            return await Result<long>.SuccessAsync(documentVersionCount);
        }
    }
}
