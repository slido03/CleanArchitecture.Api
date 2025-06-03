using CleanArchitecture.Domain.Contracts;
using CleanArchitecture.Domain.Entities.ExtendedAttributes;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities.Sgcd

{
    public class Document : AuditableEntityWithExtendedAttributes<Guid, Guid, Document, DocumentExtendedAttribute>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; } = false;
        public string URL { get; set; }
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual DocumentMatching DocumentMatching { get; set; }
        public virtual ICollection<DocumentVersion> DocumentVersions { get; } = new List<DocumentVersion>();
    }
}