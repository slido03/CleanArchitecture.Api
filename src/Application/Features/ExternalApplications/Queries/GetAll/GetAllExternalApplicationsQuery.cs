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

namespace CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll
{
    public class GetAllExternalApplicationsQuery : IRequest<PaginatedResult<GetAllExternalApplicationsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } //of the form fieldname fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllExternalApplicationsQuery(int pageNumber, int pageSize, string searchString, string orderBy)
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

    internal class GetAllExternalApplicationsQueryHandler : IRequestHandler<GetAllExternalApplicationsQuery, PaginatedResult<GetAllExternalApplicationsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllExternalApplicationsQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllExternalApplicationsResponse>> Handle(GetAllExternalApplicationsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ExternalApplication, GetAllExternalApplicationsResponse>> expression = e => new GetAllExternalApplicationsResponse
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
            };
            var appSpec = new ExternalApplicationFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<ExternalApplication>().Entities
                       .Specify(appSpec)
                       .Select(expression)
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy);
                var data = await _unitOfWork.Repository<ExternalApplication>().Entities
                       .Specify(appSpec)
                       .OrderBy(ordering)
                       .Select(expression)
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}
