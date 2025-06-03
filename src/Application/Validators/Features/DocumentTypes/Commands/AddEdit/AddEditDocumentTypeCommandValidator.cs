using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Validators.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypeCommandValidator : AbstractValidator<AddEditDocumentTypeCommand>
    {
        public AddEditDocumentTypeCommandValidator(IStringLocalizer<AddEditDocumentTypeCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request.Format)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Format is required!"]);
            RuleFor(request => request.ExternalApplication)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Application is required!"]);
        }
    }
}