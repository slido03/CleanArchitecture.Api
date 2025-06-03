using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Format { get; set; }
        [Required]
        public string ExternalApplication { get; set; }
    }

    internal class AddEditDocumentTypeCommandHandler : IRequestHandler<AddEditDocumentTypeCommand, Result<int>>
    {
        private readonly IStringLocalizer<AddEditDocumentTypeCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditDocumentTypeCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditDocumentTypeCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            command.Name = command.Name.ToUpper();
            command.ExternalApplication = command.ExternalApplication.ToUpper();

            if (await _unitOfWork.Repository<DocumentType>().Entities.Where(t => t.Id != command.Id)
                .AnyAsync(t => t.Name == command.Name, cancellationToken))
            {
                return await Result<int>.FailAsync(_localizer["Document type with this Name already exists."]);
            }

            //Get the corresponding external application
            var externalApp = await _unitOfWork.Repository<ExternalApplication>().Entities
                                    .Where(a => a.Name == command.ExternalApplication)
                                    .FirstOrDefaultAsync(cancellationToken);
            if (externalApp == null)
                return await Result<int>.FailAsync(_localizer["External Application Not Found!"]);

            if (!ApplicationConstants.Formats.DocumentFormats.CheckFormat(command.Format))
                return await Result<int>.FailAsync(_localizer["Document Type Format is not valid!"]);

            command.Format = command.Format.ToUpper();
            if (command.Id == 0)
            {
                return await AddHandler(command, externalApp.Id, cancellationToken);
            }
            else
            {
                return await EditHandler(command, externalApp.Id, cancellationToken);
            }
        }

        public async Task<Result<int>> AddHandler(AddEditDocumentTypeCommand command, int externalAppId, CancellationToken cancellationToken)
        {
            var documentType = new DocumentType
            {
                Name = command.Name,
                Description = command.Description,
                Format = command.Format,
                ExternalApplicationId = externalAppId,
            };
            await _unitOfWork.Repository<DocumentType>().AddAsync(documentType);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
            return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Saved"]);
        }

        public async Task<Result<int>> EditHandler(AddEditDocumentTypeCommand command, int externalAppId, CancellationToken cancellationToken)
        {
            var documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(command.Id);
            if (documentType != null)
            {
                documentType.Name = command.Name ?? documentType.Name;
                documentType.Description = command.Description ?? documentType.Description;
                documentType.Format = command.Format ?? documentType.Format;
                documentType.ExternalApplicationId = (externalAppId == 0) ? documentType.ExternalApplicationId : externalAppId;
                await _unitOfWork.Repository<DocumentType>().UpdateAsync(documentType);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
                return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Document Type Not Found!"]);
            }
        }
    }
}