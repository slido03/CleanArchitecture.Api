using CleanArchitecture.Domain.Contracts;
using System;

namespace CleanArchitecture.Domain.Entities.Sgcd
{
    public class DocumentVersion : AuditableEntity<Guid>
    {
        public int VersionNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
