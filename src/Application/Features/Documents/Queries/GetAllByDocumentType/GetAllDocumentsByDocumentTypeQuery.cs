using CleanArchitecture.Application.Extensions;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
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

namespace CleanArchitecture.Application.Features.Documents.Queries.GetAllByDocumentType
{
    public class GetAllDocumentsByDocumentTypeQuery : IRequest<PaginatedResult<GetAllDocumentsByDocumentTypeResponse>>
    {
        public int DocumentTypeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDocumentsByDocumentTypeQuery(int documentTypeId, int pageNumber, int pageSize, string searchString, string orderBy)
        {
            DocumentTypeId = documentTypeId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllDocumentsByDocumentTypeIdQueryHandler : IRequestHandler<GetAllDocumentsByDocumentTypeQuery, PaginatedResult<GetAllDocumentsByDocumentTypeResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        private readonly ICurrentUserService _currentUserService;

        public GetAllDocumentsByDocumentTypeIdQueryHandler(IUnitOfWork<Guid> unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetAllDocumentsByDocumentTypeResponse>> Handle(GetAllDocumentsByDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Document, GetAllDocumentsByDocumentTypeResponse>> expression = e => new GetAllDocumentsByDocumentTypeResponse
            {
                Id = e.Id,
                Title = e.Title,
                CreatedBy = e.CreatedBy,
                IsPublic = e.IsPublic,
                CreatedOn = e.CreatedOn,
                Description = e.Description,
                URL = e.URL,
                DocumentTypeId = e.DocumentTypeId,
                DocumentType = e.DocumentType.Name,
                ExternalId = e.DocumentMatching.ExternalId,
                ExternalApplication = e.DocumentType.ExternalApplication.Name,
            };
            var docSpec = new DocumentFilterSpecification(request.SearchString, _currentUserService.UserId);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Document>().Entities
                            .Where(d => d.DocumentTypeId == request.DocumentTypeId)
                            .Specify(docSpec)
                            .Select(expression)
                            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<Document>().Entities
                            .Where(d => d.DocumentTypeId == request.DocumentTypeId)
                            .Specify(docSpec)
                            .OrderBy(ordering)
                            .Select(expression)
                            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}
