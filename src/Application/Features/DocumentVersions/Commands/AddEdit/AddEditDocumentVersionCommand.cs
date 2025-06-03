using AutoMapper;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentVersions.Commands.AddEdit
{
    public class AddEditDocumentVersionCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        [Required]
        public int VersionNumber { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public Guid DocumentId { get; set; }
    }

    internal class AddEditDocumentVersionCommandHandler : IRequestHandler<AddEditDocumentVersionCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditDocumentVersionCommandHandler> _localizer;
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public AddEditDocumentVersionCommandHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper, IStringLocalizer<AddEditDocumentVersionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(AddEditDocumentVersionCommand command, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Repository<Document>().GetByIdAsync(command.DocumentId);
            if (document == null)
            {
                return await Result<Guid>.FailAsync(_localizer["No document found for this document id!"]);
            }

            if (await _unitOfWork.Repository<DocumentVersion>().Entities.Where(v => v.Id != command.Id)
                .AnyAsync(v => v.VersionNumber == command.VersionNumber, cancellationToken))
            {
                return await Result<Guid>.FailAsync(_localizer["Document Version with this version number already exists."]);
            }

            if (command.Id == Guid.Empty)
            {
                return await AddHandler(command, cancellationToken);
            }
            else
            {
                return await EditHandler(command, cancellationToken);
            }
        }

        public async Task<Result<Guid>> AddHandler(AddEditDocumentVersionCommand command, CancellationToken cancellationToken)
        {
            var documentVersion = _mapper.Map<DocumentVersion>(command);
            await _unitOfWork.Repository<DocumentVersion>().AddAsync(documentVersion);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentVersionsCacheKey);
            return await Result<Guid>.SuccessAsync(documentVersion.Id, _localizer["Document Version Saved"]);
        }

        public async Task<Result<Guid>> EditHandler(AddEditDocumentVersionCommand command, CancellationToken cancellationToken)
        {
            var documentVersion = await _unitOfWork.Repository<DocumentVersion>().GetByIdAsync(command.Id);
            if (documentVersion != null)
            {
                documentVersion.VersionNumber = (command.VersionNumber == 0) ? documentVersion.VersionNumber : command.VersionNumber;
                documentVersion.Title = command.Title ?? _localizer["New Document Version"];
                documentVersion.Description = command.Description ?? _localizer["New Document Version"];
                documentVersion.FilePath = command.FilePath ?? documentVersion.FilePath;
                documentVersion.DocumentId = (command.DocumentId == Guid.Empty) ? documentVersion.DocumentId : command.DocumentId;
                await _unitOfWork.Repository<DocumentVersion>().UpdateAsync(documentVersion);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentVersionsCacheKey);
                return await Result<Guid>.SuccessAsync(documentVersion.Id, _localizer["Document Version Updated"]);
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Version Not Found!"]);
            }
        }
    }
}
