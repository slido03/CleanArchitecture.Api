using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.ExternalApplications.Queries.GetById
{
    public class GetExternalApplicationByIdQuery : IRequest<Result<GetExternalApplicationByIdResponse>>
    {
        public int Id { get; set; }

        public GetExternalApplicationByIdQuery(int id)
        {
            Id = id;
        }
    }

    internal class GetExternalApplicationByIdQueryHandler : IRequestHandler<GetExternalApplicationByIdQuery, Result<GetExternalApplicationByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetExternalApplicationByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetExternalApplicationByIdResponse>> Handle(GetExternalApplicationByIdQuery query, CancellationToken cancellationToken)
        {
            var externalApplication = await _unitOfWork.Repository<ExternalApplication>().GetByIdAsync(query.Id);
            var mappedExternalApplication = _mapper.Map<GetExternalApplicationByIdResponse>(externalApplication);
            return await Result<GetExternalApplicationByIdResponse>.SuccessAsync(mappedExternalApplication);
        }
    }
}
