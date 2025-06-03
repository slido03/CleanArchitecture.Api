using CleanArchitecture.Application.Extensions;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Specifications.Sgcd;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.ExternalApplications.Queries.Export
{
    public class ExportExternalApplicationsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }
        public ExportExternalApplicationsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportExternalApplicationsQueryHandler : IRequestHandler<ExportExternalApplicationsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportExternalApplicationsQueryHandler> _localizer;

        public ExportExternalApplicationsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportExternalApplicationsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportExternalApplicationsQuery request, CancellationToken cancellationToken)
        {
            var externalApplicationFilterSpec = new ExternalApplicationFilterSpecification(request.SearchString);
            var externalApplications = await _unitOfWork.Repository<ExternalApplication>().Entities
                .Specify(externalApplicationFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(externalApplications, mappers: new Dictionary<string, Func<ExternalApplication, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Description"], item => item.Description },
            }, sheetName: _localizer["External Applications"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
