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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Commands.AddEdit
{
    public class AddEditDocumentCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; } = false;
        [Required]
        public string URL { get; set; }
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string ExternalId { get; set; }
        public UploadRequest UploadRequest { get; set; }
    }

    internal partial class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork<Guid> _documentUnitOfWork;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditDocumentCommandHandler> _localizer;

        public AddEditDocumentCommandHandler(IUnitOfWork<Guid> documentUnitOfWork, IUnitOfWork<int> unitOfWork, IUploadService uploadService, IStringLocalizer<AddEditDocumentCommandHandler> localizer)
        {
            _documentUnitOfWork = documentUnitOfWork;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(AddEditDocumentCommand command, CancellationToken cancellationToken)
        {
            command.DocumentType = command.DocumentType.ToUpper();

            //Check the Document Type Name
            if (string.IsNullOrWhiteSpace(command.DocumentType))
                return await Result<Guid>.FailAsync(_localizer["Document Type Name is required to Add/Update a Document!"]);

            //Get the document type corresponding
            var documentType = await _unitOfWork.Repository<DocumentType>().Entities
                                        .Where(t => t.Name == command.DocumentType)
                                        .FirstOrDefaultAsync(cancellationToken);
            if ((documentType == null))
                return await Result<Guid>.FailAsync(_localizer["Document Type Not Found!"]);

            var externalApplication = documentType.ExternalApplication;

            if (command.UploadRequest != null)
            {
                //Fill upload request app name and document type name
                command.UploadRequest.ExternalApplication = externalApplication.Name;
                command.UploadRequest.DocumentType = documentType.Name;

                //Check the document upload request extension
                command.UploadRequest.Extension = command.UploadRequest.Extension.ToLower();
                var documentExtension = ApplicationConstants.Formats.DocumentFormats.GetMatchingExtension(documentType.Format);
                if (!command.UploadRequest.Extension.Equals(documentExtension))
                    return await Result<Guid>.FailAsync(_localizer["The Document Extension doesn't match to the Document Type Format!"]);
            }
            if (command.Id == Guid.Empty)
            {
                return await AddDocumentHandler(command, documentType.Id, cancellationToken);
            }
            else
            {
                return await EditDocumentHandler(command, documentType.Id, cancellationToken);
            }
        }
    }
}