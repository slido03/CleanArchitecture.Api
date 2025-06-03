using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.ExternalApplications.Commands.Delete
{
    public class DeleteExternalApplicationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        internal class DeleteExternalApplicationCommandHandler : IRequestHandler<DeleteExternalApplicationCommand, Result<int>>
        {
            private readonly IDocumentTypeRepository _documentTypeRepository;
            private readonly IStringLocalizer<DeleteExternalApplicationCommandHandler> _localizer;
            private readonly IUnitOfWork<int> _unitOfWork;

            public DeleteExternalApplicationCommandHandler(IUnitOfWork<int> unitOfWork, IDocumentTypeRepository documentTypeRepository, IStringLocalizer<DeleteExternalApplicationCommandHandler> localizer)
            {
                _unitOfWork = unitOfWork;
                _documentTypeRepository = documentTypeRepository;
                _localizer = localizer;
            }

            public async Task<Result<int>> Handle(DeleteExternalApplicationCommand command, CancellationToken cancellationToken)
            {
                var isExternalApplicationUsed = await _documentTypeRepository.IsExternalApplicationUsed(command.Id);
                if (!isExternalApplicationUsed)
                {
                    var externalApplication = await _unitOfWork.Repository<ExternalApplication>().GetByIdAsync(command.Id);
                    if (externalApplication != null)
                    {
                        await _unitOfWork.Repository<ExternalApplication>().DeleteAsync(externalApplication);
                        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllExternalApplicationsCacheKey);
                        return await Result<int>.SuccessAsync(externalApplication.Id, _localizer["Application externe supprimée"]);
                    }
                    else
                    {
                        return await Result<int>.FailAsync(_localizer["Application externe introuvable !"]);
                    }
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Suppression non authorisée"]);
                }
            }
        }
    }
}
