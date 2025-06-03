using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentMatchings.Commands.Delete
{
    public class DeleteDocumentMatchingCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    internal class DeleteDocumentMatchingCommandHandler : IRequestHandler<DeleteDocumentMatchingCommand, Result<Guid>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IStringLocalizer<DeleteDocumentMatchingCommandHandler> _localizer;

        public DeleteDocumentMatchingCommandHandler(IUnitOfWork<Guid> unitOfWork, IStringLocalizer<DeleteDocumentMatchingCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(DeleteDocumentMatchingCommand command, CancellationToken cancellationToken)
        {
            var documentMatching = await _unitOfWork.Repository<DocumentMatching>().GetByIdAsync(command.Id);
            if (documentMatching != null)
            {
                await _unitOfWork.Repository<DocumentMatching>().DeleteAsync(documentMatching);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<Guid>.SuccessAsync(documentMatching.Id, _localizer["Document Matching Deleted"]);
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Matching Not Found!"]);
            }
        }
    }
}
