using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById
{
    public class GetDocumentVersionByIdQuery : IRequest<Result<GetDocumentVersionByIdResponse>>
    {
        public Guid Id { get; set; }

        public GetDocumentVersionByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetDocumentVersionByIdQueryHandler : IRequestHandler<GetDocumentVersionByIdQuery, Result<GetDocumentVersionByIdResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDocumentVersionByIdQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetDocumentVersionByIdResponse>> Handle(GetDocumentVersionByIdQuery query, CancellationToken cancellationToken)
        {
           var docVersion = await _unitOfWork.Repository<DocumentVersion>().GetByIdAsync(query.Id);
            var mappedDocVersion = _mapper.Map<GetDocumentVersionByIdResponse>(docVersion);
            return await Result<GetDocumentVersionByIdResponse>.SuccessAsync(mappedDocVersion);
        }
    }
}
