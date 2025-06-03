using CleanArchitecture.Application.Features.DocumentVersions.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Validators.Features.DocumentVersions.Commands.AddEdit
{
    public class AddEditDocumentVersionCommandValidator : AbstractValidator<AddEditDocumentVersionCommand>
    {
        public AddEditDocumentVersionCommandValidator(IStringLocalizer<AddEditDocumentVersionCommandValidator> localizer)
        {
            RuleFor(request => request.VersionNumber)
                .GreaterThan(0).WithMessage(x => localizer["Version Number is required!"]);
            RuleFor(request => request.Title)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Title is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request.FilePath)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Document Version File is required!"]);
            RuleFor(request => request.DocumentId)
                .NotEmpty().WithMessage(x => localizer["Document is required!"]);
        }
    }
}
