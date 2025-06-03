using CleanArchitecture.Application.Models.Sgcd;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Validators.Models.Sgcd
{
    public class ImportDocumentTypesValidator : AbstractValidator<ImportDocumentTypes>
    {
        public ImportDocumentTypesValidator(IStringLocalizer<ImportDocumentTypesValidator> localizer)
        {
            RuleFor(request => request.Name)
               .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request.Format)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Format is required!"]);
            RuleFor(request => request.ExternalApplicationId)
                .GreaterThan(0).WithMessage(x => localizer["Application Id cannot be negative!"]);
        }
    }
}
