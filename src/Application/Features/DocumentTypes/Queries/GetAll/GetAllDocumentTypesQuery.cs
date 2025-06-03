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

namespace CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll
{
    public class GetAllDocumentTypesQuery : IRequest<PaginatedResult<GetAllDocumentTypesResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDocumentTypesQuery(int pageNumber, int pageSize, string searchString, string orderBy)
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

    internal class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, PaginatedResult<GetAllDocumentTypesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllDocumentTypesResponse>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DocumentType, GetAllDocumentTypesResponse>> expression = e => new GetAllDocumentTypesResponse
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Format = e.Format,
                ExternalApplicationId = e.ExternalApplicationId,
                ExternalApplication = e.ExternalApplication.Name,
            };
            var docTypeSpec = new DocumentTypeFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<DocumentType>().Entities
                        .Specify(docTypeSpec)
                        .Select(expression)
                        .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<DocumentType>().Entities
                       .Specify(docTypeSpec)
                       .OrderBy(ordering)
                       .Select(expression)
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}