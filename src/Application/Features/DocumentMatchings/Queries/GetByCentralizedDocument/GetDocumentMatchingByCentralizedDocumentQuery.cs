using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetByCentralizedDocument
{
    public class GetDocumentMatchingByCentralizedDocumentQuery : IRequest<Result<GetDocumentMatchingByCentralizedDocumentResponse>>
    {
        public Guid CentralizedDocumentId { get; set; }

        public GetDocumentMatchingByCentralizedDocumentQuery(Guid centralizedDocumentId)
        {
            CentralizedDocumentId = centralizedDocumentId;
        }
    }

    internal class GetDocumentMatchingByCentralizedDocumentQueryHandler : IRequestHandler<GetDocumentMatchingByCentralizedDocumentQuery, Result<GetDocumentMatchingByCentralizedDocumentResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDocumentMatchingByCentralizedDocumentQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetDocumentMatchingByCentralizedDocumentResponse>> Handle(GetDocumentMatchingByCentralizedDocumentQuery query, CancellationToken cancellationToken)
        {
            var docMatching = await _unitOfWork.Repository<DocumentMatching>().Entities
                 .Where(m => m.CentralizedDocumentId == query.CentralizedDocumentId)
                 .FirstOrDefaultAsync(cancellationToken);
            var mappedDocMatching = _mapper.Map<GetDocumentMatchingByCentralizedDocumentResponse>(docMatching);
            return await Result<GetDocumentMatchingByCentralizedDocumentResponse>.SuccessAsync(mappedDocMatching);
        }
    }
}
