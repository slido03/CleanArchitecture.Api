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

namespace CleanArchitecture.Application.Features.Documents.Queries.GetAll
{
    public class GetAllDocumentsQuery : IRequest<PaginatedResult<GetAllDocumentsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDocumentsQuery(int pageNumber, int pageSize, string searchString, string orderBy)
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

    internal class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, PaginatedResult<GetAllDocumentsResponse>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetAllDocumentsQueryHandler(IUnitOfWork<Guid> unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetAllDocumentsResponse>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Document, GetAllDocumentsResponse>> expression = e => new GetAllDocumentsResponse
            {
                Id = e.Id,
                Title = e.Title,
                CreatedBy = e.CreatedBy,
                IsPublic = e.IsPublic,
                CreatedOn = e.CreatedOn,
                Description = e.Description,
                URL = e.URL,
                ExternalApplication = e.DocumentType.ExternalApplication.Name,
                DocumentType = e.DocumentType.Name,
                ExternalId = e.DocumentMatching.ExternalId
            };
            var docSpec = new DocumentFilterSpecification(request.SearchString, _currentUserService.UserId);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Document>().Entities
                            .Specify(docSpec)
                            .Select(expression)
                            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<Document>().Entities
                               .Specify(docSpec)
                               .OrderBy(ordering)
                               .Select(expression)
                               .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}