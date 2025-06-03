using AutoMapper;
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

namespace CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit
{
    public class AddEditExternalApplicationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    internal class AddEditExternalApplicationCommandHandler : IRequestHandler<AddEditExternalApplicationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditExternalApplicationCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditExternalApplicationCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditExternalApplicationCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditExternalApplicationCommand command, CancellationToken cancellationToken)
        {
            command.Name = command.Name.ToUpper();
            if (await _unitOfWork.Repository<ExternalApplication>().Entities.Where(a => a.Id != command.Id)
                .AnyAsync(a => a.Name == command.Name, cancellationToken))
            {
                return await Result<int>.FailAsync(_localizer["External Application with this Name already exists."]);
            }
            if (command.Id == 0)
            {
                return await AddHandler(command, cancellationToken);
            }
            else
            {
                return await EditHandler(command, cancellationToken);
            }
        }

        public async Task<Result<int>> AddHandler(AddEditExternalApplicationCommand command, CancellationToken cancellationToken)
        {
            var externalApplication = _mapper.Map<ExternalApplication>(command);
            await _unitOfWork.Repository<ExternalApplication>().AddAsync(externalApplication);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllExternalApplicationsCacheKey);
            return await Result<int>.SuccessAsync(externalApplication.Id, _localizer["External Application Saved"]);
        }

        public async Task<Result<int>> EditHandler(AddEditExternalApplicationCommand command, CancellationToken cancellationToken)
        {
            var externalApplication = await _unitOfWork.Repository<ExternalApplication>().GetByIdAsync(command.Id);
            if (externalApplication != null)
            {
                externalApplication.Name = command.Name ?? externalApplication.Name;
                externalApplication.Description = command.Description ?? externalApplication.Description;
                await _unitOfWork.Repository<ExternalApplication>().UpdateAsync(externalApplication);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllExternalApplicationsCacheKey);
                return await Result<int>.SuccessAsync(externalApplication.Id, _localizer["External Application Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["External Application Not Found!"]);
            }
        }
    }
}
