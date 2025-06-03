using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId
{
    public class GetDocumentByExternalIdQuery : IRequest<Result<GetDocumentByExternalIdResponse>>
    {
        public string DocumentType { get; set; }
        public string ExternalId { get; set; }

        public GetDocumentByExternalIdQuery(string documentType, string externalId)
        {
            DocumentType = documentType;
            ExternalId = externalId;
        }
    }

    internal class GetDocumentByExternalIdQueryHandler : IRequestHandler<GetDocumentByExternalIdQuery, Result<GetDocumentByExternalIdResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<GetDocumentByExternalIdQueryHandler> _localizer;

        public GetDocumentByExternalIdQueryHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper, IStringLocalizer<GetDocumentByExternalIdQueryHandler> localizer, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<Result<GetDocumentByExternalIdResponse>> Handle(GetDocumentByExternalIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.DocumentType) || string.IsNullOrWhiteSpace(request.ExternalId))
                return await Result<GetDocumentByExternalIdResponse>.FailAsync(_localizer["There's no document found for those entries."]);

            request.DocumentType = request.DocumentType.ToUpper();
            var document = await _unitOfWork.Repository<Document>().Entities
                .Where(d => d.DocumentType.Name == request.DocumentType)
                .Where(d => d.DocumentMatching.ExternalId == request.ExternalId)
                .FirstOrDefaultAsync(cancellationToken);
            if (document == null)
            {
                return await Result<GetDocumentByExternalIdResponse>.FailAsync(_localizer["There's no document found for those entries."]);
            }
            else
            {
                if ((document.IsPublic == true) || (document.IsPublic == false && document.CreatedBy == _currentUserService.UserId))
                {
                    var mappedDoc = _mapper.Map<GetDocumentByExternalIdResponse>(document);
                    return await Result<GetDocumentByExternalIdResponse>.SuccessAsync(mappedDoc);
                }
                else
                {
                    return await Result<GetDocumentByExternalIdResponse>.FailAsync(_localizer["You cannot view this document"]);
                }
            }
        }
    }
}
