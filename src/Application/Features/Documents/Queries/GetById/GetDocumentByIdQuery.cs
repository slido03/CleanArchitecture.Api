using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Queries.GetById
{
    public class GetDocumentByIdQuery : IRequest<Result<GetDocumentByIdResponse>>
    {
        public Guid Id { get; set; }

        public GetDocumentByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<GetDocumentByIdResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<GetDocumentByIdQueryHandler> _localizer;

        public GetDocumentByIdQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper, ICurrentUserService currentUserService, IStringLocalizer<GetDocumentByIdQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<Result<GetDocumentByIdResponse>> Handle(GetDocumentByIdQuery query, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Repository<Document>().GetByIdAsync(query.Id);
            if (document != null)
            {
                if ((document.IsPublic == true) || (document.IsPublic == false && document.CreatedBy == _currentUserService.UserId))
                {
                    var mappedDocument = _mapper.Map<GetDocumentByIdResponse>(document);
                    return await Result<GetDocumentByIdResponse>.SuccessAsync(mappedDocument);
                }
                else
                {
                    return await Result<GetDocumentByIdResponse>.FailAsync(_localizer["You cannot view this document"]);
                }
            }
            return await Result<GetDocumentByIdResponse>.SuccessAsync(new GetDocumentByIdResponse());
        }
    }
}