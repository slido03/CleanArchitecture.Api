using System;

namespace CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById
{
    public class GetDocumentMatchingByIdResponse
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public Guid CentralizedDocumentId { get; set; }
        public string CentralizedDocument { get; set; }
    }
}
