using CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Validators.Features.DocumentMatchings.Commands.AddEdit
{
    public class AddEditDocumentMatchingCommandValidator : AbstractValidator<AddEditDocumentMatchingCommand>
    {
        public AddEditDocumentMatchingCommandValidator(IStringLocalizer<AddEditDocumentMatchingCommandValidator> localizer)
        {
            RuleFor(request => request.ExternalId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["External Id is required!"]);
            RuleFor(request => request.CentralizedDocumentId)
                .NotEmpty().WithMessage(x => localizer["Centralized Document is required!"]);

        }
    }
}
