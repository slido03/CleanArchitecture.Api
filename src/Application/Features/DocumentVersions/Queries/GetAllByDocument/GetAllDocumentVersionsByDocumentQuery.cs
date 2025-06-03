using CleanArchitecture.Application.Extensions;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Specifications.Sgcd;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAllByDocument
{
    public class GetAllDocumentVersionsByDocumentQuery : IRequest<PaginatedResult<GetAllDocumentVersionsByDocumentResponse>>
    {
        public Guid DocumentId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDocumentVersionsByDocumentQuery(Guid documentId, int pageNumber, int pageSize, string searchString, string orderBy) 
        {
            DocumentId = documentId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllDocumentVersionsByDocumentQueryHandler : IRequestHandler<GetAllDocumentVersionsByDocumentQuery, PaginatedResult<GetAllDocumentVersionsByDocumentResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetAllDocumentVersionsByDocumentQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllDocumentVersionsByDocumentResponse>> Handle(GetAllDocumentVersionsByDocumentQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DocumentVersion, GetAllDocumentVersionsByDocumentResponse>> expression = e => new GetAllDocumentVersionsByDocumentResponse
            {
                Id = e.Id,
                VersionNumber = e.VersionNumber,
                Title = e.Title,
                Description = e.Description,
                FilePath = e.FilePath,
                CreatedOn = e.CreatedOn,
                DocumentId = e.DocumentId,
                Document = e.Document.Title,
            };
            var docVersionSpec = new DocumentVersionFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<DocumentVersion>().Entities
                                .Where(v => v.DocumentId == request.DocumentId)
                                .Specify(docVersionSpec)
                                .Select(expression)
                                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<DocumentVersion>().Entities
                                .Where(v => v.DocumentId == request.DocumentId)
                                .Specify(docVersionSpec)
                                .OrderBy(ordering)
                                .Select(expression)
                                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}
