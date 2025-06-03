using CleanArchitecture.Domain.Contracts;
using System;

namespace CleanArchitecture.Domain.Entities.Sgcd

{
    public class DocumentMatching : AuditableEntity<Guid>
    {
        public string ExternalId { get; set; }
        public Guid CentralizedDocumentId { get; set; }
        public virtual Document CentralizedDocument { get; set; }
    }
}
