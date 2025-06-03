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

namespace CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit
{
    public class AddEditDocumentMatchingCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        [Required]
        public string ExternalId { get; set; }
        [Required]
        public Guid CentralizedDocumentId { get; set; }
    }

    internal class AddEditDocumentMatchingCommandHandler : IRequestHandler<AddEditDocumentMatchingCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditDocumentMatchingCommandHandler> _localizer;
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public AddEditDocumentMatchingCommandHandler(IUnitOfWork<Guid> unitOfWork, IMapper mapper, IStringLocalizer<AddEditDocumentMatchingCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(AddEditDocumentMatchingCommand command, CancellationToken cancellationToken)
        {
            var centralizedDocument = await _unitOfWork.Repository<Document>().GetByIdAsync(command.CentralizedDocumentId);
            if (centralizedDocument == null)
            {
                return await Result<Guid>.FailAsync(_localizer["No document found for this centralized document id!"]);
            }
            if (await _unitOfWork.Repository<DocumentMatching>().Entities.Where(m => m.Id != command.Id)
                .AnyAsync(m => m.CentralizedDocumentId == command.CentralizedDocumentId, cancellationToken))
            {
                return await Result<Guid>.FailAsync(_localizer["Document Matching for this document already exists."]);
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

        public async Task<Result<Guid>> AddHandler(AddEditDocumentMatchingCommand command, CancellationToken cancellationToken)
        {
            var documentMatching = _mapper.Map<DocumentMatching>(command);
            await _unitOfWork.Repository<DocumentMatching>().AddAsync(documentMatching);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentMatchingsCacheKey);
            return await Result<Guid>.SuccessAsync(documentMatching.Id, _localizer["Document Matching Saved"]);
        }

        public async Task<Result<Guid>> EditHandler(AddEditDocumentMatchingCommand command, CancellationToken cancellationToken)
        {
            var documentMatching = await _unitOfWork.Repository<DocumentMatching>().GetByIdAsync(command.Id);
            if (documentMatching != null)
            {
                documentMatching.ExternalId = command.ExternalId ?? documentMatching.ExternalId;
                documentMatching.CentralizedDocumentId = (command.CentralizedDocumentId == Guid.Empty) ? documentMatching.CentralizedDocumentId : command.CentralizedDocumentId;
                await _unitOfWork.Repository<DocumentMatching>().UpdateAsync(documentMatching);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentMatchingsCacheKey);
                return await Result<Guid>.SuccessAsync(documentMatching.Id, _localizer["Document Matching Updated"]);
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Matching Not Found!"]);
            }
        }
    }
}
