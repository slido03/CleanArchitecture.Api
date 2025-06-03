using CleanArchitecture.Domain.Entities.ExtendedAttributes;
using CleanArchitecture.Domain.Entities.Sgcd;
using Microsoft.Extensions.Localization;
using System;

namespace CleanArchitecture.Application.Validators.Features.ExtendedAttributes.Commands.AddEdit
{
    public class AddEditDocumentExtendedAttributeCommandValidator : AddEditExtendedAttributeCommandValidator<Guid, Guid, Document, DocumentExtendedAttribute>
    {
        public AddEditDocumentExtendedAttributeCommandValidator(IStringLocalizer<AddEditExtendedAttributeCommandValidatorLocalization> localizer) : base(localizer)
        {
            // you can override the validation rules here
        }
    }
}