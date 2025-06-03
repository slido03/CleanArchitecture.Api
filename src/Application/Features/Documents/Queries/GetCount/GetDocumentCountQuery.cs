using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Queries.GetCount
{
    public class GetDocumentCountQuery : IRequest<Result<long>>
    {
        public GetDocumentCountQuery()
        {
        }
    }

    internal class GetDocumentCountQueryHandler : IRequestHandler<GetDocumentCountQuery, Result<long>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetDocumentCountQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(GetDocumentCountQuery request, CancellationToken cancellationToken)
        {
            var documentCount = await _unitOfWork.Repository<Document>().GetLongCountAsync();
            return await Result<long>.SuccessAsync(documentCount);
        }
    }
}
