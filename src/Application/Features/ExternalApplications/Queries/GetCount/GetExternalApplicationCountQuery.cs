using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.ExternalApplications.Queries.GetCount
{
    public class GetExternalApplicationCountQuery : IRequest<Result<int>>
    {
        public GetExternalApplicationCountQuery()
        {
        }
    }

    internal class GetExternalApplicationCountQueryHandler : IRequestHandler<GetExternalApplicationCountQuery, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetExternalApplicationCountQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetExternalApplicationCountQuery request, CancellationToken cancellationToken)
        {
            var externalApplicationCount = await _unitOfWork.Repository<ExternalApplication>().GetCountAsync();
            return await Result<int>.SuccessAsync(externalApplicationCount);
        }
    }
}
