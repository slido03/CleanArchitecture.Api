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

namespace CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll
{
    public class GetAllDocumentMatchingsQuery : IRequest<PaginatedResult<GetAllDocumentMatchingsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDocumentMatchingsQuery(int pageNumber, int pageSize, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllDocumentMatchingsQueryHandler : IRequestHandler<GetAllDocumentMatchingsQuery, PaginatedResult<GetAllDocumentMatchingsResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetAllDocumentMatchingsQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllDocumentMatchingsResponse>> Handle(GetAllDocumentMatchingsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DocumentMatching, GetAllDocumentMatchingsResponse>> expression = e => new GetAllDocumentMatchingsResponse
            {
                Id = e.Id,
                CentralizedDocumentId = e.CentralizedDocumentId,
                ExternalId = e.ExternalId,
                CentralizedDocument = e.CentralizedDocument.Title
            };
            var docMatchingSpec = new DocumentMatchingFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<DocumentMatching>().Entities
                            .Specify(docMatchingSpec)
                            .Select(expression)
                            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<DocumentMatching>().Entities
                           .Specify(docMatchingSpec)
                           .OrderBy(ordering)
                           .Select(expression)
                           .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}
