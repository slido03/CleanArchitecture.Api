using System.Collections.Generic;

namespace CleanArchitecture.Application.Features.Dashboards.Queries.GetData
{
    public class GetDashboardDataResponse
    {
        public int ExternalApplicationCount { get; set; }
        public int DocumentTypeCount { get; set; }
        public long DocumentCount { get; set; }
        public long DocumentMatchingCount { get; set; }
        public long DocumentVersionCount { get; set; }
        public int UserCount { get; set; }
        public int RoleCount { get; set; }

        public List<ChartSeries> DataEnterBarChart { get; set; } = new();
        public Dictionary<string, double> DocumentsByDocumentTypePieChart { get; set; } = new();
    }

    public class ChartSeries
    {
        public ChartSeries() { }

        public string Name { get; set; }
        public double[] Data { get; set; }
    }
}