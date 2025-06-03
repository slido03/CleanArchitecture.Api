using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCount
{
    public class GetDocumentTypeCountQuery : IRequest<Result<int>>
    {
        public GetDocumentTypeCountQuery()
        {
        }
    }

    internal class GetDocumentTypeCountQueryHandler : IRequestHandler<GetDocumentTypeCountQuery, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetDocumentTypeCountQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetDocumentTypeCountQuery request, CancellationToken cancellationToken)
        {
            var documentTypeCount = await _unitOfWork.Repository<DocumentType>().GetCountAsync();
            return await Result<int>.SuccessAsync(documentTypeCount);
        }
    }
}
