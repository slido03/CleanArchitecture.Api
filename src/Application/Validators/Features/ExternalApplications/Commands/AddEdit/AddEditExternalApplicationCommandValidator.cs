using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Validators.Features.ExternalApplications.Commands.AddEdit
{
    public class AddEditExternalApplicationCommandValidator : AbstractValidator<AddEditExternalApplicationCommand>
    {
        public AddEditExternalApplicationCommandValidator(IStringLocalizer<AddEditExternalApplicationCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
        }
    }
}
