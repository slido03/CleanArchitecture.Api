using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Commands.AddEdit
{
    internal partial class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> AddDocumentHandler(AddEditDocumentCommand command, int documentTypeId, CancellationToken cancellationToken)
        {
            //Check the external Id
            if (string.IsNullOrWhiteSpace(command.ExternalId))
                return await Result<Guid>.FailAsync(_localizer["You should specify an external Id!"]);

            var uploadRequest = command.UploadRequest;
            var doc = new Document
            {
                Title = command.Title,
                Description = command.Description,
                IsPublic = command.IsPublic,
                URL = command.URL,
                DocumentTypeId = documentTypeId,
            };
            if (uploadRequest != null)
            {
                uploadRequest.FileName = $"D-{Guid.NewGuid()}";
                uploadRequest.VersionNumber = 1;
                doc.URL = _uploadService.Upload(uploadRequest);
            }
            await _documentUnitOfWork.Repository<Document>().AddAsync(doc);
            await _documentUnitOfWork.Commit(cancellationToken);

            //add the corresponding DocumentMatching and DocumentVersion
            await _documentUnitOfWork.Repository<DocumentMatching>().AddAsync(
                new DocumentMatching
                {
                    ExternalId = command.ExternalId,
                    CentralizedDocumentId = doc.Id
                });
            if ((uploadRequest != null) && (!string.IsNullOrWhiteSpace(doc.URL)))
            {
                await _documentUnitOfWork.Repository<DocumentVersion>().AddAsync(
                new DocumentVersion
                {
                    VersionNumber = 1,
                    Title = command.Title,
                    Description = command.Description,
                    DocumentId = doc.Id,
                    FilePath = doc.URL
                });
            }
            await _documentUnitOfWork.Commit(cancellationToken);
            return await Result<Guid>.SuccessAsync(doc.Id, _localizer["Document Saved"]);
        }
    }
}
