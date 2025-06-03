using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Commands.Delete
{
    public class DeleteDocumentCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    internal class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<Guid>>
    {
        private readonly IDocumentMatchingRepository _documentMatchingRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IFileDeletionService _fileDeletionService;
        private readonly IStringLocalizer<DeleteDocumentCommandHandler> _localizer;

        public DeleteDocumentCommandHandler(IUnitOfWork<Guid> unitOfWork, IFileDeletionService fileDeletionService, IDocumentMatchingRepository documentMatchingRepository, IDocumentVersionRepository documentVersionRepository, IStringLocalizer<DeleteDocumentCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _fileDeletionService = fileDeletionService;
            _documentMatchingRepository = documentMatchingRepository;
            _documentVersionRepository = documentVersionRepository;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
        {
            var documentsWithExtendedAttributes = _unitOfWork.Repository<Document>().Entities.Include(x => x.ExtendedAttributes);
            var isCentralizedDocumentUsed = await _documentMatchingRepository.IsCentralizedDocumentUsed(command.Id);
            var isDocumentUsed = await _documentVersionRepository.IsDocumentUsed(command.Id);

            if (!isCentralizedDocumentUsed && !isDocumentUsed)
            {
                var document = await _unitOfWork.Repository<Document>().GetByIdAsync(command.Id);
                if (document != null)
                {
                    await _unitOfWork.Repository<Document>().DeleteAsync(document);

                    var fileDeletionRequest = new FileDeletionRequest() { FilePath = document.URL };
                    _fileDeletionService.DeleteFile(fileDeletionRequest);

                    // delete all caches related with deleted entity
                    var cacheKeys = await documentsWithExtendedAttributes.SelectMany(x => x.ExtendedAttributes).Where(x => x.EntityId == command.Id).Distinct().Select(x => ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(nameof(Document), x.EntityId))
                        .ToListAsync(cancellationToken);
                    cacheKeys.Add(ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(nameof(Document)));
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, cacheKeys.ToArray());

                    return await Result<Guid>.SuccessAsync(document.Id, _localizer["Document Deleted"]);
                }
                else
                {
                    return await Result<Guid>.FailAsync(_localizer["Document Not Found!"]);
                }
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Deletion Not Allowed"]);
            }
        }
    }
}