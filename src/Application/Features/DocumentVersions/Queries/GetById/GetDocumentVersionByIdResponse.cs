using System;

namespace CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById
{
    public class GetDocumentVersionByIdResponse
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FilePath { get; set; }
        public Guid DocumentId { get; set; }
        public string Document { get; set; }
    }
}
