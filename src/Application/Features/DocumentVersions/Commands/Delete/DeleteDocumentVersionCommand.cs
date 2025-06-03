using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentVersions.Commands.Delete
{
    public class DeleteDocumentVersionCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    internal class DeleteDocumentVersionCommandHandler : IRequestHandler<DeleteDocumentVersionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IFileDeletionService _fileDeletionService;
        private readonly IStringLocalizer<DeleteDocumentVersionCommandHandler> _localizer;

        public DeleteDocumentVersionCommandHandler(IUnitOfWork<Guid> unitOfWork, IFileDeletionService fileDeletionService, IStringLocalizer<DeleteDocumentVersionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _fileDeletionService = fileDeletionService;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(DeleteDocumentVersionCommand command, CancellationToken cancellationToken)
        {
            var documentVersion = await _unitOfWork.Repository<DocumentVersion>().GetByIdAsync(command.Id);
            if (documentVersion != null)
            {
                //Check if it is the actual document version and if true update the document URL to string.Empty
                var document = documentVersion.Document;
                if (document != null)
                {
                    if (document.URL == documentVersion.FilePath)
                    {
                        document.URL = string.Empty;
                        await _unitOfWork.Repository<Document>().UpdateAsync(document);
                        await _unitOfWork.Commit(cancellationToken);
                    }
                }
                await _unitOfWork.Repository<DocumentVersion>().DeleteAsync(documentVersion);
                var fileDeletionRequest = new FileDeletionRequest() { FilePath = documentVersion.FilePath };
                _fileDeletionService.DeleteFile(fileDeletionRequest);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<Guid>.SuccessAsync(documentVersion.Id, _localizer["Document Version Deleted"]);
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Version Not Found!"]);
            }
        }
    }
}
