using CleanArchitecture.Domain.Contracts;
using CleanArchitecture.Domain.Entities.Sgcd;
using System;

namespace CleanArchitecture.Domain.Entities.ExtendedAttributes
{
    public class DocumentExtendedAttribute : AuditableEntityExtendedAttribute<Guid, Guid, Document>
    {
    }
}