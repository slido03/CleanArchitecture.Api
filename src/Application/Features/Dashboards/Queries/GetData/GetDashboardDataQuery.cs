using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services.Identity;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Dashboards.Queries.GetData
{
    public class GetDashboardDataQuery : IRequest<Result<GetDashboardDataResponse>>
    {
        public GetDashboardDataQuery()
        {
        }
    }

    internal class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, Result<GetDashboardDataResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUnitOfWork<Guid> _unitOfWork2;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<GetDashboardDataQueryHandler> _localizer;

        public GetDashboardDataQueryHandler(IUnitOfWork<int> unitOfWork, IUnitOfWork<Guid> unitOfWork2, IUserService userService, IRoleService roleService, IStringLocalizer<GetDashboardDataQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork2 = unitOfWork2;
            _userService = userService;
            _roleService = roleService;
            _localizer = localizer;
        }

        public async Task<Result<GetDashboardDataResponse>> Handle(GetDashboardDataQuery query, CancellationToken cancellationToken)
        {
            var response = new GetDashboardDataResponse
            {
                ExternalApplicationCount = await _unitOfWork.Repository<ExternalApplication>().GetCountAsync(),
                DocumentCount = await _unitOfWork2.Repository<Document>().GetLongCountAsync(),
                DocumentTypeCount = await _unitOfWork.Repository<DocumentType>().GetCountAsync(),
                DocumentMatchingCount = await _unitOfWork2.Repository<DocumentMatching>().GetLongCountAsync(),
                DocumentVersionCount = await _unitOfWork2.Repository<DocumentVersion>().GetLongCountAsync(),
                UserCount = await _userService.GetCountAsync(),
                RoleCount = await _roleService.GetCountAsync()
            };

            var selectedYear = DateTime.Now.Year;

            double[] externalApplicationsFigure = new double[13];
            double[] documentTypesFigure = new double[13];
            double[] documentsFigure = new double[13];
            double[] documentMatchingsFigure = new double[13];
            double[] documentVersionsFigure = new double[13];
            for (int i = 1; i <= 12; i++)
            {
                var month = i;
                var filterStartDate = new DateTime(selectedYear, month, 01);
                var filterEndDate = new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59, 59); // Monthly Based

                externalApplicationsFigure[i - 1] = await _unitOfWork.Repository<ExternalApplication>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                documentsFigure[i - 1] = await _unitOfWork2.Repository<Document>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                documentTypesFigure[i - 1] = await _unitOfWork.Repository<DocumentType>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                documentVersionsFigure[i - 1] = await _unitOfWork2.Repository<DocumentVersion>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                documentMatchingsFigure[i - 1] = await _unitOfWork2.Repository<DocumentMatching>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
            }

            var docTypes = await _unitOfWork.Repository<DocumentType>().GetAllAsync();
            if (docTypes.Count > 0)
            {
                foreach (var docType in docTypes)
                {
                    var docCount = await _unitOfWork2.Repository<Document>().Entities
                                            .Where(d => d.DocumentTypeId == docType.Id)
                                            .CountAsync(cancellationToken);
                    response.DocumentsByDocumentTypePieChart.Add(docType.Name, docCount);
                }
            }

            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["External Applications"], Data = externalApplicationsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Types"], Data = documentTypesFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Documents"], Data = documentsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Matchings"], Data = documentMatchingsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Versions"], Data = documentVersionsFigure });

            return await Result<GetDashboardDataResponse>.SuccessAsync(response);
        }
    }
}