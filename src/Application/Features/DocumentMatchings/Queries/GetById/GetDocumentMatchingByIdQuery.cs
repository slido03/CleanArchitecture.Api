using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById
{
    public class GetDocumentMatchingByIdQuery : IRequest<Result<GetDocumentMatchingByIdResponse>>
    {
        public Guid Id { get; set; }

        public GetDocumentMatchingByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetDocumentMatchingByIdQueryHandler : IRequestHandler<GetDocumentMatchingByIdQuery, Result<GetDocumentMatchingByIdResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDocumentMatchingByIdQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetDocumentMatchingByIdResponse>> Handle(GetDocumentMatchingByIdQuery query, CancellationToken cancellationToken)
        {
            var docMatching = await _unitOfWork.Repository<DocumentMatching>().GetByIdAsync(query.Id);
            var mappedDocMatching = _mapper.Map<GetDocumentMatchingByIdResponse>(docMatching);
            return await Result<GetDocumentMatchingByIdResponse>.SuccessAsync(mappedDocMatching);
        }
    }
}
