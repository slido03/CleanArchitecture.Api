using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCountByExternalApplication
{
    public class GetDocumentTypeCountByExternalApplicationQuery : IRequest<Result<int>>
    {
        public int ExternalApplicationId { get; set; }
        public GetDocumentTypeCountByExternalApplicationQuery(int externalApplicationId)
        {
            ExternalApplicationId = externalApplicationId;
        }
    }

    internal class GetDocumentTypeCountByExternalApplicationQueryHandler : IRequestHandler<GetDocumentTypeCountByExternalApplicationQuery, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetDocumentTypeCountByExternalApplicationQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetDocumentTypeCountByExternalApplicationQuery request, CancellationToken cancellationToken)
        {
            var documentTypeCount = await _unitOfWork.Repository<DocumentType>().Entities
                .Where(t => t.ExternalApplicationId == request.ExternalApplicationId)
                .CountAsync(cancellationToken);
            return await Result<int>.SuccessAsync(documentTypeCount);
        }
    }
}
