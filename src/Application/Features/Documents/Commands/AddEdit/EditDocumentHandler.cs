using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Commands.AddEdit
{
    internal partial class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> EditDocumentHandler(AddEditDocumentCommand command, int documentTypeId, CancellationToken cancellationToken)
        {
            var uploadRequest = command.UploadRequest;
            var doc = await _documentUnitOfWork.Repository<Document>().GetByIdAsync(command.Id);
            if (doc != null)
            {
                doc.Title = command.Title ?? doc.Title;
                doc.Description = command.Description ?? doc.Description;
                doc.IsPublic = command.IsPublic;
                if (uploadRequest != null)
                {
                    uploadRequest.FileName = GetFileNameFromUrl(doc.URL);
                    uploadRequest.VersionNumber = GetNextVersionNumber(doc);
                    doc.URL = _uploadService.Upload(uploadRequest);
                }
                doc.DocumentTypeId = documentTypeId;
                await _documentUnitOfWork.Repository<Document>().UpdateAsync(doc);
                await _documentUnitOfWork.Commit(cancellationToken);

                //update the corresponding DocumentMatching and add a new DocumentVersion if uploadResquest was done
                if (!string.IsNullOrWhiteSpace(command.ExternalId))
                {
                    var docMatching = doc.DocumentMatching;
                    if (docMatching != null)
                    {
                        docMatching.ExternalId = command.ExternalId;
                        docMatching.CentralizedDocumentId = doc.Id;
                        await _documentUnitOfWork.Repository<DocumentMatching>().UpdateAsync(docMatching);
                        await _documentUnitOfWork.Commit(cancellationToken);
                    }
                    else
                    {
                        await _documentUnitOfWork.Repository<DocumentMatching>().AddAsync(
                            new DocumentMatching
                            {
                                ExternalId = command.ExternalId,
                                CentralizedDocumentId = doc.Id
                            });
                        await _documentUnitOfWork.Commit(cancellationToken);
                    }
                }
                if ((uploadRequest != null) && (!string.IsNullOrWhiteSpace(doc.URL)))
                {
                    await _documentUnitOfWork.Repository<DocumentVersion>().AddAsync(
                        new DocumentVersion
                        {
                            VersionNumber = uploadRequest.VersionNumber,
                            Title = command.Title,
                            Description = command.Description,
                            DocumentId = doc.Id,
                            FilePath = doc.URL
                        });
                    await _documentUnitOfWork.Commit(cancellationToken);
                }
                return await Result<Guid>.SuccessAsync(doc.Id, _localizer["Document Updated"]);
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Not Found!"]);
            }
        }

        public static string GetFileNameFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return $"D-{Guid.NewGuid()}";
            var fullFileName = Path.GetFileName(url);
            if (!Path.HasExtension(url))
                return fullFileName;

            var extension = Path.GetExtension(url);
            var fileName = fullFileName.Replace(extension, string.Empty);
            return fileName;
        }

        public static int GetNextVersionNumber(Document document)
        {
            if (document == null)
                return 0;

            var docVersions = document.DocumentVersions.AsQueryable();
            var count = docVersions.Count();
            if (count == 0)
            {
                return 1;
            }
            var lastDocVersion = docVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            var nextVersionNumber = lastDocVersion.VersionNumber + 1;
            return nextVersionNumber;
        }
    }
}
