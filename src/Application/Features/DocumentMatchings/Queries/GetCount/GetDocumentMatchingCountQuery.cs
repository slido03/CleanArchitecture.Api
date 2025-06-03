using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetCount
{
    public class GetDocumentMatchingCountQuery : IRequest<Result<long>>
    {
        public GetDocumentMatchingCountQuery()
        {
        }
    }

    internal class GetDocumentMatchingCountQueryHandler : IRequestHandler<GetDocumentMatchingCountQuery, Result<long>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetDocumentMatchingCountQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(GetDocumentMatchingCountQuery request, CancellationToken cancellationToken)
        {
            var documentMatchingCount = await _unitOfWork.Repository<DocumentMatching>().GetLongCountAsync();
            return await Result<long>.SuccessAsync(documentMatchingCount);
        }
    }
}
